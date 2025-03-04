using FeatureBasedFolderStructure.Domain.Common;

namespace FeatureBasedFolderStructure.Domain.Entities.Users;

public class Role : BaseAuditableEntity<int>
{
    public string Name { get; set; } = null!;
    public string NormalizedName { get; set; } = null!;
    public string? Description { get; set; }
    
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RoleClaim> RoleClaims { get; set; } = new List<RoleClaim>();
}