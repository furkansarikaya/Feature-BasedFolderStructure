namespace FeatureBasedFolderStructure.Application.Common.Models;

public class ApiMetadata
{
    public int? TotalCount { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public int? TotalPages { get; set; }
    public string? RequestId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}