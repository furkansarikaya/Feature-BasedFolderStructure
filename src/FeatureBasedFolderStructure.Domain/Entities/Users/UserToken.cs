using FeatureBasedFolderStructure.Domain.Enums;
using FS.EntityFramework.Library.Common;

namespace FeatureBasedFolderStructure.Domain.Entities.Users;

public sealed class UserToken : BaseAuditableEntity<int>
{
    public Guid UserId { get; set; }
    public TokenType TokenType { get; set; }
    public string TokenValue { get; set; } = null!;
    public DateTime? ExpiryDate { get; set; }
    
    public ApplicationUser User { get; set; } = null!;
}