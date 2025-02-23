using System.Net;
using FeatureBasedFolderStructure.Application.Common.Exceptions;
using FeatureBasedFolderStructure.Application.Common.Models;

namespace FeatureBasedFolderStructure.API.Common;

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
            logger.LogError(exception, "Validation hatası oluştu");

            var response = BaseResponse<object>.ErrorResult(
                "Validation hatası oluştu", 
                exception.Errors.Values.SelectMany(x => x).ToList()
            );
            
            context.Response.StatusCode = (int)response.StatusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(response);
        }
        catch (NotFoundException exception)
        {
            logger.LogError(exception, "Kayıt bulunamadı");

            var response = BaseResponse<object>.ErrorResult(
                "Kayıt bulunamadı", 
                [exception.Message], 
                HttpStatusCode.NotFound
            );
            
            context.Response.StatusCode = (int)response.StatusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(response);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Beklenmeyen bir hata oluştu");

            var response = BaseResponse<object>.ErrorResult("Beklenmeyen bir hata oluştu", [exception.Message], HttpStatusCode.InternalServerError);

            context.Response.StatusCode = (int)response.StatusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}