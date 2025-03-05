using FeatureBasedFolderStructure.Domain.Common.Interfaces;
using FeatureBasedFolderStructure.Domain.Entities.Users;
using FeatureBasedFolderStructure.Domain.Enums;

namespace FeatureBasedFolderStructure.Domain.Interfaces.Users;

public interface IApplicationUserRepository : IRepository<ApplicationUser, Guid>
{
    Task<ApplicationUser?> GetUserWithRolesAndClaims(Guid userId);
    Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<ApplicationUser>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}