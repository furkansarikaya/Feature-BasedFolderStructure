using FS.EntityFramework.Library.Common;

namespace FeatureBasedFolderStructure.Domain.Entities.Users;

public sealed class UserRole : BaseAuditableEntity<int>
{
    public Guid UserId { get; set; }
    public int RoleId { get; set; }
    
    public ApplicationUser User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}