using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Domain.Entities.Users;
using FeatureBasedFolderStructure.Domain.Enums;

namespace FeatureBasedFolderStructure.Application.Interfaces.Users;

public interface IApplicationUserService
{
    Task<BaseResponse<ApplicationUser>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BaseResponse<ApplicationUser>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<BaseResponse<IEnumerable<ApplicationUser>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<BaseResponse<IEnumerable<ApplicationUser>>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default);
    Task<BaseResponse<Guid>> CreateAsync(ApplicationUser user, string password, CancellationToken cancellationToken = default);
    Task<BaseResponse<bool>> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken = default);
    Task<BaseResponse<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    BaseResponse<bool> VerifyPassword(ApplicationUser user, string password);
    Task<BaseResponse<bool>> ChangePasswordAsync(Guid id, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
    Task<BaseResponse<bool>> LockUserAsync(Guid id, DateTime lockoutEnd, CancellationToken cancellationToken = default);
    Task<BaseResponse<bool>> UnlockUserAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BaseResponse<bool>> ChangeUserStatusAsync(Guid id, UserStatus status, CancellationToken cancellationToken = default);
}