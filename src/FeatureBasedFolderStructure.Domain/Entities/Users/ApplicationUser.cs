using FeatureBasedFolderStructure.Domain.Common;
using FeatureBasedFolderStructure.Domain.Enums;

namespace FeatureBasedFolderStructure.Domain.Entities.Users;

public class ApplicationUser: BaseAuditableEntity<Guid>
{
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public UserStatus Status { get; set; } = UserStatus.Active;
    
    public bool EmailConfirmed { get; set; }
    public DateTime? LockoutEnd { get; set; }
    public int AccessFailedCount { get; set; }
    
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<UserToken> UserTokens { get; set; } = new List<UserToken>();
}