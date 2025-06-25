using FeatureBasedFolderStructure.Domain.Entities.Users;
using FeatureBasedFolderStructure.Domain.Enums;
using FS.EntityFramework.Library.Interfaces;

namespace FeatureBasedFolderStructure.Domain.Interfaces.Users;

public interface IApplicationUserRepository : IRepository<ApplicationUser, Guid>
{
    Task<ApplicationUser?> GetUserWithRolesAndClaims(Guid userId, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<ApplicationUser>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}