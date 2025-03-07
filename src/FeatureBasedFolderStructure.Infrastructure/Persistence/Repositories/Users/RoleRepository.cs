using FeatureBasedFolderStructure.Domain.Entities.Users;
using FeatureBasedFolderStructure.Domain.Interfaces.Users;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FeatureBasedFolderStructure.Infrastructure.Persistence.Repositories;

public class RoleRepository(ApplicationDbContext context) : BaseRepository<Role, int>(context), IRoleRepository
{

    public async Task<Role?> GetByNameAsync(string roleName)
    {
        return await AsQueryable()
            .FirstOrDefaultAsync(r => r.NormalizedName.Equals(roleName, StringComparison.InvariantCultureIgnoreCase));
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
        return await AsQueryable()
            .AnyAsync(r => r.NormalizedName.Equals(roleName, StringComparison.InvariantCultureIgnoreCase));
    }

    public async Task<IEnumerable<RoleClaim>> GetRoleClaimsAsync(int roleId)
    {
        return await context.RoleClaims
            .Where(rc => rc.RoleId == roleId)
            .ToListAsync();
    }
}