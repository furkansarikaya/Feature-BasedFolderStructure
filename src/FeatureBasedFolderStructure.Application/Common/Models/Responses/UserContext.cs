namespace FeatureBasedFolderStructure.Application.Common.Models.Responses;

/// <summary>
/// User context metadata.
/// </summary>
public class UserContext
{
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public string[]? Roles { get; set; }
    public string? TenantId { get; set; }
}