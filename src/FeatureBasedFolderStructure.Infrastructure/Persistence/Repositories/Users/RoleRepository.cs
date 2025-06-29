using FeatureBasedFolderStructure.Domain.Entities.Users;
using FeatureBasedFolderStructure.Domain.Interfaces.Users;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Context;
using FS.AutoServiceDiscovery.Extensions.Attributes;
using FS.EntityFramework.Library.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureBasedFolderStructure.Infrastructure.Persistence.Repositories.Users;

[ServiceRegistration(ServiceLifetime.Scoped, Order = 1)]
public class RoleRepository(ApplicationDbContext context) : BaseRepository<Role, int>(context), IRoleRepository
{

    public async Task<Role?> GetByNameAsync(string roleName)
    {
        return await FirstOrDefaultAsync(r => r.NormalizedName == roleName);
    }

    public async Task<IEnumerable<Role>> GetRolesByUserIdAsync(Guid userId)
    {
        return await context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .Select(ur => ur.Role)
            .ToListAsync();
    }

    public async Task<bool> RoleExistsAsync(string roleName)
    {
        return await ExistsAsync(r => r.NormalizedName == roleName);
    }

    public async Task<IEnumerable<RoleClaim>> GetRoleClaimsAsync(int roleId)
    {
        return await context.RoleClaims
            .Where(rc => rc.RoleId == roleId)
            .ToListAsync();
    }
}