using FeatureBasedFolderStructure.Domain.Common;
using FeatureBasedFolderStructure.Domain.Enums;

namespace FeatureBasedFolderStructure.Domain.Entities.Users;

public class RoleClaim : BaseAuditableEntity<int>
{
    public int RoleId { get; set; }
    public ClaimType ClaimType { get; set; }
    public string ClaimValue { get; set; } = null!;
    
    public Role Role { get; set; } = null!;
}