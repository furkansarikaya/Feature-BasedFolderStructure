using System.Diagnostics;
using System.Reflection;
using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Application.Common.Models.Responses;
using FeatureBasedFolderStructure.Application.Interfaces.Users;
using FeatureBasedFolderStructure.Domain.Common.Paging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FeatureBasedFolderStructure.API.Filters;

/// <summary>
/// Response wrapping action filter.
/// Bu filter tüm controller action'larında automatic çalışıyor ve metadata inject ediyor.
/// 
/// Working Principle:
/// 1. Action execute olmadan önce timestamp ve stopwatch başlatılıyor
/// 2. Action execute olduktan sonra result wrap ediliyor
/// 3. Metadata HTTP context'ten extract edilip inject ediliyor
/// </summary>
public class ApiResponseWrapperFilter(
    ICurrentUserService currentUserService,
    ILogger<ApiResponseWrapperFilter> logger,
    IApplicationUserService applicationUserService,
    IDateTime dateTime)
    : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Pre-execution: Start timing and setup tracking
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString();
        var correlationId = GetCorrelationId(context.HttpContext);

        // Store timing info in HttpContext for access in post-execution
        context.HttpContext.Items["RequestStartTime"] = dateTime.Now;
        context.HttpContext.Items["RequestId"] = requestId;
        context.HttpContext.Items["CorrelationId"] = correlationId;
        context.HttpContext.Items["Stopwatch"] = stopwatch;

        logger.LogDebug("Request started: {RequestId} - {Method} {Path}",
            requestId, context.HttpContext.Request.Method, context.HttpContext.Request.Path);

        // Execute the action
        var executedContext = await next();

        // Post-execution: Wrap response and inject metadata
        stopwatch.Stop();

        if (ShouldWrapResponse(executedContext))
        {
            await WrapResponse(executedContext, stopwatch.ElapsedMilliseconds);
        }

        logger.LogDebug("Request completed: {RequestId} in {ElapsedMs}ms",
            requestId, stopwatch.ElapsedMilliseconds);
    }

    /// <summary>
    /// Response wrap edilip edilmeyeceğini determine eder.
    /// </summary>
    private static bool ShouldWrapResponse(ActionExecutedContext context)
    {
        // Error response'ları wrap etme
        if (context.Exception != null) return false;

        // File download/stream response'ları wrap etme
        if (context.Result is FileResult) return false;

        // Redirect response'ları wrap etme
        if (context.Result is RedirectResult or RedirectToActionResult) return false;

        // Zaten wrapped response'ları tekrar wrap etme
        if (IsAlreadyWrapped(context.Result)) return false;

        // API controller'lar için wrap et
        return context.Controller.GetType().GetCustomAttribute<ApiControllerAttribute>() != null;
    }

    /// <summary>
    /// Response'ın zaten wrap edilip edilmediğini check eder.
    /// </summary>
    private static bool IsAlreadyWrapped(IActionResult? result)
    {
        if (result is ObjectResult objectResult)
        {
            return objectResult.Value?.GetType().IsGenericType == true &&
                   objectResult.Value.GetType().GetGenericTypeDefinition() == typeof(ApiResponse<>);
        }

        return false;
    }

    /// <summary>
    /// Actual response wrapping logic - BURADA metadata injection oluyor.
    /// </summary>
    private async Task WrapResponse(ActionExecutedContext context, long executionTimeMs)
    {
        if (context.Result is ObjectResult { Value: not null } objectResult)
        {
            // Original response data'yı al
            var originalData = objectResult.Value;

            // Metadata build et - CORE LOGIC BURADA
            var metadata = await BuildMetadata(context, executionTimeMs, originalData);

            // ApiResponse wrapper oluştur
            var wrappedResponse = CreateWrappedResponse(originalData, metadata);

            switch (context.Result)
            {
                case CreatedAtActionResult createdResult:
                    // CreatedResult için wrapped response'u set et
                    context.Result = new CreatedAtActionResult(
                        createdResult.ActionName,
                        createdResult.ControllerName,
                        createdResult.RouteValues,
                        wrappedResponse)
                    {
                        StatusCode = 201, // Created status code
                        ContentTypes = { "application/json" },
                        DeclaredType = wrappedResponse.GetType()
                    };
                    break;
                case ObjectResult objectResultResponse:
                    context.Result = new ObjectResult(wrappedResponse)
                    {
                        StatusCode = objectResult.StatusCode,
                        ContentTypes = objectResult.ContentTypes,
                        DeclaredType = wrappedResponse.GetType()
                    };
                    break;
                default:
                    // Diğer result türleri için özel handling gerekebilir
                    logger.LogWarning("Unhandled result type: {ResultType}", context.Result.GetType().Name);
                    break;
            }
           
        }
    }

    /// <summary>
    /// Metadata building - AUTOMATIC INJECTION'ın core logic'i.
    /// Bu method HTTP context'ten tüm metadata'yı extract ediyor.
    /// </summary>
    private async Task<ResponseMetadata> BuildMetadata(
        ActionExecutedContext context,
        long executionTimeMs,
        object originalData)
    {
        var httpContext = context.HttpContext;
        var request = httpContext.Request;

        var metadata = new ResponseMetadata
        {
            // Basic request information
            RequestId = httpContext.Items["RequestId"]?.ToString() ?? "",
            CorrelationId = httpContext.Items["CorrelationId"]?.ToString(),
            Timestamp = (DateTime)(httpContext.Items["RequestStartTime"] ?? dateTime.Now),
            ExecutionTimeMs = executionTimeMs,
            Path = request.Path.Value ?? "",
            Method = request.Method,

            // API version (header'dan veya config'den)
            Version = GetApiVersion(httpContext),

            // User context
            User = await BuildUserContext(),

            // Pagination metadata (eğer result paged ise)
            Pagination = ExtractPaginationMetadata(originalData),

            // Query metadata (eğer varsa)
            Query = ExtractQueryMetadata(httpContext),

            // Additional metadata
            Additional = ExtractAdditionalMetadata(httpContext)
        };

        return metadata;
    }

    /// <summary>
    /// User context building.
    /// </summary>
    private async Task<UserContext?> BuildUserContext()
    {
        if (!Guid.TryParse(currentUserService.UserId, out var userId))
            return null;

        var user = await applicationUserService.GetUserWithRolesAndClaims(userId);

        return new UserContext
        {
            UserId = user.Id.ToString(),
            Email = user.Email,
            Roles = user.UserRoles.Select(r => r.Role.Name).ToArray(),
            // TenantId = currentUserService.User?.FindFirst("tenant_id")?.Value
        };
    }

    /// <summary>
    /// Pagination metadata extraction.
    /// Bu method paged result'lardan automatic pagination metadata extract ediyor.
    /// </summary>
    private static PaginationMetadata? ExtractPaginationMetadata(object data)
    {
        if (data is IPagedResult pagedResult)
        {
            return new PaginationMetadata
            {
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                TotalPages = pagedResult.TotalPages,
                TotalItems = pagedResult.TotalItems,
                HasNextPage = pagedResult.HasNextPage,
                HasPreviousPage = pagedResult.HasPreviousPage
            };
        }

        return null;
    }

    /// <summary>
    /// Query metadata extraction.
    /// HttpContext'ten database query statistics extract ediyor.
    /// </summary>
    private static QueryMetadata? ExtractQueryMetadata(HttpContext httpContext)
    {
        // EF Core interceptor'lardan query stats alınabilir
        var queryStats = httpContext.Items["QueryStats"] as Dictionary<string, object>;

        if (queryStats != null)
        {
            return new QueryMetadata
            {
                DatabaseQueriesCount = (int)(queryStats.GetValueOrDefault("QueriesCount", 0)),
                DatabaseExecutionTimeMs = (long)(queryStats.GetValueOrDefault("ExecutionTimeMs", 0L)),
                CacheHits = (int)(queryStats.GetValueOrDefault("CacheHits", 0)),
                CacheMisses = (int)(queryStats.GetValueOrDefault("CacheMisses", 0)),
                ExecutedQueries = queryStats.GetValueOrDefault("ExecutedQueries") as string[]
            };
        }

        return null;
    }

    /// <summary>
    /// Additional metadata extraction.
    /// Custom metadata'lar için extensible pattern.
    /// </summary>
    private static Dictionary<string, object>? ExtractAdditionalMetadata(HttpContext httpContext)
    {
        var additional = new Dictionary<string, object>();

        // Request size
        if (httpContext.Request.ContentLength.HasValue)
        {
            additional["RequestSizeBytes"] = httpContext.Request.ContentLength.Value;
        }

        // User agent
        var userAgent = httpContext.Request.Headers["User-Agent"].FirstOrDefault();
        if (!string.IsNullOrEmpty(userAgent))
        {
            additional["UserAgent"] = userAgent;
        }

        // Client IP
        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString();
        if (!string.IsNullOrEmpty(clientIp))
        {
            additional["ClientIP"] = clientIp;
        }

        // Custom headers
        var customHeaders = httpContext.Request.Headers
            .Where(h => h.Key.StartsWith("X-Custom-"))
            .ToDictionary(h => h.Key, h => (object)h.Value.ToString());

        foreach (var header in customHeaders)
        {
            additional[header.Key] = header.Value;
        }

        return additional.Any() ? additional : null;
    }
    
    /// <summary>
    /// Core transformation logic - BURADA pagination bilgileri SİLİNİYOR.
    /// </summary>
    private (object transformedData, Type transformedDataType) TransformPagedResult(object originalData, Type dataType)
    {
        if(!IsPagedResult(originalData))
            return (originalData, dataType);
        
        // Extract items from PagedResult<T>
        var itemsProperty = dataType.GetProperty("Items");
        if (itemsProperty == null)
        {
            logger.LogWarning("Items property not found on type {DataType}", dataType.Name);
            return (originalData, dataType);
        }
        
        var items = itemsProperty.GetValue(originalData);
        if (items == null)
        {
            logger.LogWarning("Items collection is null for type {DataType}", dataType.Name);
            return (originalData, dataType);
        }
        
        // Determine item type from PagedResult<T>
        var itemType = GetItemTypeFromPagedResult(dataType);
        if (itemType == null)
        {
            logger.LogWarning("Could not determine item type from {DataType}", dataType.Name);
            return (originalData, dataType);
        }
        
        // Create CleanPagedResult<T> - NO pagination fields
        var cleanResultType = typeof(CleanPagedResult<>).MakeGenericType(itemType);
        var cleanResult = Activator.CreateInstance(cleanResultType);
        
        if (cleanResult == null)
        {
            logger.LogError("Failed to create clean result type for {ItemType}", itemType.Name);
            return (originalData, dataType);
        }
        
        // Set only Items property - pagination bilgileri EXCLUDE
        var cleanItemsProperty = cleanResultType.GetProperty("Items");
        cleanItemsProperty?.SetValue(cleanResult, items);
        
        logger.LogInformation("Successfully transformed PagedResult<{ItemType}> to CleanPagedResult<{ItemType}>", 
            itemType.Name, itemType.Name);
        
        return (cleanResult, cleanResultType);
    }

    /// <summary>
    /// Wrapped response creation.
    /// Original data'yı ApiResponse wrapper'a sarıyor.
    /// </summary>
    private object CreateWrappedResponse(object originalData, ResponseMetadata metadata)
    {
        var dataType = originalData.GetType();
        var (transformedData, transformedDataType) = TransformPagedResult(originalData, dataType);
        
        var responseType = typeof(ApiResponse<>).MakeGenericType(transformedDataType);

        // ApiResponse instance oluştur
        var response = Activator.CreateInstance(responseType);
        // Set properties with type-safe assignments
        var dataProperty = responseType.GetProperty("Data");
        var successProperty = responseType.GetProperty("Success");
        var metadataProperty = responseType.GetProperty("Metadata");

        if (dataProperty == null || successProperty == null || metadataProperty == null)
        {
            throw new InvalidOperationException($"ApiResponse<{transformedDataType.Name}> missing required properties");
        }

        // Type-safe property assignment
        dataProperty.SetValue(response, transformedData); // No type mismatch now!
        successProperty.SetValue(response, true);
        metadataProperty.SetValue(response, metadata);

        logger.LogDebug("Successfully created ApiResponse<{TransformedType}>", transformedDataType.Name);

        return response!;
    }

    /// <summary>
    /// Correlation ID extraction.
    /// </summary>
    private static string GetCorrelationId(HttpContext httpContext)
    {
        return httpContext.Request.Headers["X-Correlation-ID"].FirstOrDefault()
               ?? httpContext.TraceIdentifier
               ?? Guid.NewGuid().ToString();
    }

    /// <summary>
    /// API version extraction.
    /// </summary>
    private static string GetApiVersion(HttpContext httpContext)
    {
        return httpContext.Request.Headers["X-API-Version"].FirstOrDefault()
               ?? httpContext.Request.Query["version"].FirstOrDefault()
               ?? "1.0";
    }
    
    /// <summary>
    /// PagedResult instance detection.
    /// </summary>
    private static bool IsPagedResult(object data)
    {
        return data is IPagedResult;
    }
    
    /// <summary>
    /// Extract item type from PagedResult<T>.
    /// </summary>
    private static Type? GetItemTypeFromPagedResult(Type pagedResultType)
    {
        if (!pagedResultType.IsGenericType) return null;
        var genericArgs = pagedResultType.GetGenericArguments();
        return genericArgs.Length > 0 ? genericArgs[0] : // T from PagedResult<T>
            null;
    }
}