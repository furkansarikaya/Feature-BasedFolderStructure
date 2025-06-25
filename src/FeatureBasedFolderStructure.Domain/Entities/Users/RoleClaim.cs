using FeatureBasedFolderStructure.Domain.Enums;
using FS.EntityFramework.Library.Common;

namespace FeatureBasedFolderStructure.Domain.Entities.Users;

public sealed class RoleClaim : BaseAuditableEntity<int>
{
    public int RoleId { get; set; }
    public ClaimType ClaimType { get; set; }
    public string ClaimValue { get; set; } = null!;
    
    public Role Role { get; set; } = null!;
}