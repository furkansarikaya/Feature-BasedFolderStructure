namespace FeatureBasedFolderStructure.Application.Common.Models.Responses;

/// <summary>
/// Pagination metadata for paged results.
/// </summary>
public class PaginationMetadata
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}