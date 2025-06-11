using System.Net;
using FeatureBasedFolderStructure.Application.Common.Exceptions;
using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Domain.Common.Attributes;

namespace FeatureBasedFolderStructure.API.Common;

[ServiceRegistration(ServiceLifetime.Scoped, Order = -1)]
public class GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException exception)
        {
            await HandleExceptionAsync(context, exception, "Validation hatası oluştu", exception.Errors.Values.SelectMany(x => x).ToList(), HttpStatusCode.BadRequest);
        }
        catch (NotFoundException exception)
        {
            await HandleExceptionAsync(context, exception, "Kayıt bulunamadı", [exception.Message], HttpStatusCode.NotFound);
        }
        catch(ForbiddenAccessException exception)
        {
            await HandleExceptionAsync(context, exception, "Yetkiniz yok", [exception.Message], HttpStatusCode.Forbidden);
        }
        catch(UnauthorizedAccessException exception)
        {
            await HandleExceptionAsync(context, exception, "Yetkiniz yok", [exception.Message], HttpStatusCode.Unauthorized);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception, "Beklenmeyen bir hata oluştu", [exception.Message], HttpStatusCode.InternalServerError);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, string message, List<string> errors, HttpStatusCode statusCode)
    {
        logger.LogError(exception, message);

        var response = BaseResponse<object>.ErrorResult(message, errors, statusCode);

        context.Response.StatusCode = (int)response.StatusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(response);
    }
}