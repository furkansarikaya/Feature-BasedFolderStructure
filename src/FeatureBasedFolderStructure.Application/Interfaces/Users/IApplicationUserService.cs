using FeatureBasedFolderStructure.Domain.Entities.Users;
using FeatureBasedFolderStructure.Domain.Enums;

namespace FeatureBasedFolderStructure.Application.Interfaces.Users;

public interface IApplicationUserService
{
    Task<ApplicationUser> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApplicationUser> GetUserWithRolesAndClaims(Guid id, CancellationToken cancellationToken = default);
    Task<ApplicationUser> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<ApplicationUser>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ApplicationUser>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(ApplicationUser user, string password, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    bool VerifyPassword(ApplicationUser user, string password);
    Task<bool> ChangePasswordByForgetPasswordAsync(Guid id, string newPassword, CancellationToken cancellationToken = default);
    Task<bool> ChangePasswordAsync(Guid id, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
    Task<bool> LockUserAsync(Guid id, DateTime lockoutEnd, CancellationToken cancellationToken = default);
    Task<bool> UnlockUserAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ChangeUserStatusAsync(Guid id, UserStatus status, CancellationToken cancellationToken = default);
}