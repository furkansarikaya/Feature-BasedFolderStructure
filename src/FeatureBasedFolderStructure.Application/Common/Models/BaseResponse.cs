using System.Net;

namespace FeatureBasedFolderStructure.Application.Common.Models;

public class BaseResponse<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
    public List<string> Errors { get; private set; }
    public HttpStatusCode StatusCode { get; set; }

    public BaseResponse()
    {
        Message = string.Empty;
        Errors = [];
        Success = true;
        StatusCode = HttpStatusCode.OK;
    }

    public static BaseResponse<T> SuccessResult(T data, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new BaseResponse<T>
        {
            Data = data,
            StatusCode = statusCode
        };
    }

    public static BaseResponse<T> ErrorResult(string message, List<string> errors, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new BaseResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors,
            StatusCode = statusCode
        };
    }

    public static BaseResponse<T> NotFound(string message = "Record not found", string error = "")
    {
        return new BaseResponse<T>
        {
            Success = false,
            Message = message,
            Errors = [error],
            StatusCode = HttpStatusCode.NotFound
        };
    }
}