namespace FeatureBasedFolderStructure.Application.Common.Models.Responses;

/// <summary>
/// Query execution metadata.
/// </summary>
public class QueryMetadata
{
    public int DatabaseQueriesCount { get; set; }
    public long DatabaseExecutionTimeMs { get; set; }
    public int CacheHits { get; set; }
    public int CacheMisses { get; set; }
    public string[]? ExecutedQueries { get; set; }
}