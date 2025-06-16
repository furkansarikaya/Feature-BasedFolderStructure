using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Domain.Common.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FeatureBasedFolderStructure.API.Filters;

/// <summary>
/// Bu action filter tüm controller action'larının response'larını otomatik olarak
/// ApiResponse formatına wrap eder. Bu sayede her endpoint'te manuel wrapping 
/// yapmak zorunda kalmazsınız.
/// 
/// Mental Model: Bu filter bir "decorator pattern" implementation'ıdır.
/// Original response'ı alır, onu ApiResponse container'ına koyar ve client'a gönderir.
/// </summary>
[ServiceRegistration(ServiceLifetime.Scoped, Order = 200)]
public class AutoApiResponseWrapperFilter(ILogger<AutoApiResponseWrapperFilter> logger) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Action execution'dan önce - pre-processing
        var skipWrapper = ShouldSkipWrapper(context);
        
        if (skipWrapper.ShouldSkip)
        {
            logger.LogDebug("Skipping API response wrapper for {Action}. Reason: {Reason}", 
                context.ActionDescriptor.DisplayName, skipWrapper.Reason);
            
            // Wrapper'ı skip et, normal flow'u devam ettir
            await next();
            return;
        }
        
        // Action'ı execute et
        var executedContext = await next();
        
        // Action execution'dan sonra - post-processing
        if (executedContext.Exception == null)
        {
            // Success case - response'ı wrap et
            await WrapSuccessResponse(executedContext);
        }
        // Exception case zaten Global Exception Handler tarafından handle ediliyor
        // Bu filter sadece success case'leri handle eder
    }
    
    /// <summary>
    /// Bu method wrapper'ın skip edilip edilmeyeceğini belirler.
    /// Decision tree:
    /// 1. Action'da SkipApiResponseWrapper attribute'u var mı?
    /// 2. Controller'da SkipApiResponseWrapper attribute'u var mı?
    /// 3. Response type special type mı? (FileResult, RedirectResult, etc.)
    /// </summary>
    private (bool ShouldSkip, string Reason) ShouldSkipWrapper(ActionExecutingContext context)
    {
        // 1. Action level'da skip attribute kontrolü
        var actionSkipAttribute = context.ActionDescriptor.EndpointMetadata
            .OfType<SkipApiResponseWrapperAttribute>()
            .FirstOrDefault();
            
        if (actionSkipAttribute != null)
        {
            return (true, $"Action level skip: {actionSkipAttribute.Reason}");
        }
        
        // 2. Controller level'da skip attribute kontrolü
        var controllerSkipAttribute = context.Controller.GetType()
            .GetCustomAttributes(typeof(SkipApiResponseWrapperAttribute), true)
            .Cast<SkipApiResponseWrapperAttribute>()
            .FirstOrDefault();
            
        if (controllerSkipAttribute != null)
        {
            return (true, $"Controller level skip: {controllerSkipAttribute.Reason}");
        }
        
        // 3. Special response type kontrolü (runtime'da belirlenecek)
        // Bu kısım OnActionExecuted'da yapılacak
        
        return (false, "");
    }
    
    /// <summary>
    /// Success response'ları ApiResponse formatına wrap eder.
    /// Bu method'un mathematical beauty'si şurada: 
    /// Original response type'ına bakmaksızın generic wrapping yapıyor.
    /// </summary>
    private async Task WrapSuccessResponse(ActionExecutedContext context)
    {
        var result = context.Result;
        
        // Zaten ApiResponse formatında ise double-wrapping yapma
        if (IsAlreadyWrapped(result))
        {
            logger.LogDebug("Response is already wrapped, skipping auto-wrapper");
            return;
        }
        
        // Special result type'ları check et
        if (IsSpecialResultType(result))
        {
            logger.LogDebug("Special result type detected: {ResultType}, skipping wrapper", 
                result?.GetType().Name);
            return;
        }
        
        // Response'ı wrap et
        var wrappedResponse = CreateWrappedResponse(result, context);
        context.Result = wrappedResponse;
        
        logger.LogDebug("Successfully wrapped response for {Action}", 
            context.ActionDescriptor.DisplayName);
    }
    
    /// <summary>
    /// Response'ın zaten wrap edilip edilmediğini kontrol eder.
    /// </summary>
    private bool IsAlreadyWrapped(IActionResult? result)
    {
        if (result is ObjectResult objectResult)
        {
            // Generic type check - reflection kullanarak ApiResponse<T> olup olmadığını kontrol et
            var valueType = objectResult.Value?.GetType();
            if (valueType != null && valueType.IsGenericType)
            {
                var genericTypeDef = valueType.GetGenericTypeDefinition();
                return genericTypeDef == typeof(ApiResponse<>);
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Wrapping yapılmaması gereken special result type'ları belirler.
    /// Bu method'da "open-closed principle" uygulanmış - 
    /// yeni type'lar eklemek için modify değil extend yapılıyor.
    /// </summary>
    private bool IsSpecialResultType(IActionResult? result)
    {
        return result switch
        {
            // File operations
            FileResult => true,
            
            // Redirects
            RedirectResult => true,
            RedirectToActionResult => true,
            RedirectToRouteResult => true,
            LocalRedirectResult => true,
            
            // Status code results without content
            StatusCodeResult => true,
            EmptyResult => true,
            
            // Partial views (MVC scenarios)
            PartialViewResult => true,
            ViewResult => true,
            
            // Custom special results (extensible)
            ISpecialResult => true,
            
            _ => false
        };
    }
    
    /// <summary>
    /// Original result'ı ApiResponse container'ına wrap eder.
    /// Bu method'un core responsibility'si type-safe wrapping yapmak.
    /// </summary>
    private ObjectResult CreateWrappedResponse(IActionResult? originalResult, ActionExecutedContext context)
    {
        object? data = null;
        var statusCode = 200;
        string? message = null;
        
        // Original result'dan data ve status code'u extract et
        switch (originalResult)
        {
            case JsonResult jsonResult:
                data = jsonResult.Value;
                statusCode = jsonResult.StatusCode ?? 200;
                break;
                
            case OkResult:
                statusCode = 200;
                message = "Success";
                break;
                
            case OkObjectResult okObjectResult:
                data = okObjectResult.Value;
                statusCode = 200;
                break;
                
            case CreatedResult createdResult:
                data = createdResult.Value;
                statusCode = 201;
                message = "Created successfully";
                break;
                
            case CreatedAtActionResult createdAtActionResult:
                data = createdAtActionResult.Value;
                statusCode = 201;
                message = "Created successfully";
                break;
                
            case AcceptedResult:
                statusCode = 202;
                message = "Accepted";
                break;
                
            case NoContentResult:
                statusCode = 204;
                message = "No content";
                break;
                
            default:
                // Unknown result type - try to extract value via reflection
                data = TryExtractValueViaReflection(originalResult);
                break;
        }
        
        // ApiResponse oluştur - generic type inference kullanarak
        var apiResponse = CreateTypedApiResponse(data, message, context);
        
        return new ObjectResult(apiResponse)
        {
            StatusCode = statusCode
        };
    }
    
    /// <summary>
    /// Type-safe ApiResponse oluşturur.
    /// Generic type system'i kullanarak compile-time type safety sağlar.
    /// </summary>
    private object CreateTypedApiResponse(object? data, string? message, ActionExecutedContext context)
    {
        if (data == null)
        {
            return ApiResponse<object>.SuccessResult(null, message ?? "Success");
        }
        
        // Reflection ile generic method invoke et
        var dataType = data.GetType();
        var method = typeof(ApiResponse<>)
            .MakeGenericType(dataType)
            .GetMethod(nameof(ApiResponse<object>.SuccessResult), 
                new[] { dataType, typeof(string) });
                
        return method!.Invoke(null, new object[] { data, message ?? "Success" })!;
    }
    
    /// <summary>
    /// Unknown result type'lar için reflection ile value extract etmeye çalışır.
    /// Fallback mechanism - defensive programming approach.
    /// </summary>
    private object? TryExtractValueViaReflection(IActionResult? result)
    {
        if (result == null) return null;
        
        try
        {
            // "Value" property'si var mı check et
            var valueProperty = result.GetType().GetProperty("Value");
            return valueProperty?.GetValue(result);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to extract value from result type {ResultType}", 
                result.GetType().Name);
            return null;
        }
    }
}