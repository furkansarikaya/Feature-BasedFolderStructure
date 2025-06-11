using FeatureBasedFolderStructure.Domain.Common.Attributes;
using FeatureBasedFolderStructure.Domain.Entities.Users;
using FeatureBasedFolderStructure.Domain.Enums;
using FeatureBasedFolderStructure.Domain.Interfaces.Users;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureBasedFolderStructure.Infrastructure.Persistence.Repositories.Users;

[ServiceRegistration(ServiceLifetime.Scoped, Order = 1)]
public class ApplicationUserRepository(ApplicationDbContext context) : BaseRepository<ApplicationUser, Guid>(context), IApplicationUserRepository
{
    public async Task<ApplicationUser?> GetUserWithRolesAndClaims(Guid userId)
    {
        return await GetQueryable()
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r.RoleClaims)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await FirstOrDefaultAsync(predicate: e => e.Email == email, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<ApplicationUser>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default)
    {
        return await GetWithIncludesAsync(
            predicate: e => e.Status == status,
            cancellationToken: cancellationToken
        );
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await ExistsAsync(e => e.Email == email, cancellationToken);
    }
}