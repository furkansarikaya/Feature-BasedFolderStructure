using FeatureBasedFolderStructure.Domain.Common;

namespace FeatureBasedFolderStructure.Domain.Entities.Users;

public class UserRole : BaseAuditableEntity<int>
{
    public Guid UserId { get; set; }
    public int RoleId { get; set; }
    
    public ApplicationUser User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}