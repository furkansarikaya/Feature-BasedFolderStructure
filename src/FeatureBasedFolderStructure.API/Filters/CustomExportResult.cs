using Microsoft.AspNetCore.Mvc;

namespace FeatureBasedFolderStructure.API.Filters;

/// <summary>
/// Custom result example - wrapping'den exempt olmak için ISpecialResult implement ediyor.
/// </summary>
public class CustomExportResult : ActionResult, ISpecialResult
{
    private readonly byte[] _data;
    private readonly string _fileName;
    private readonly string _contentType;
    
    public CustomExportResult(byte[] data, string fileName, string contentType)
    {
        _data = data;
        _fileName = fileName;
        _contentType = contentType;
    }
    
    public override async Task ExecuteResultAsync(ActionContext context)
    {
        var response = context.HttpContext.Response;
        response.ContentType = _contentType;
        response.Headers.Add("Content-Disposition", $"attachment; filename={_fileName}");
        
        await response.Body.WriteAsync(_data);
    }
}