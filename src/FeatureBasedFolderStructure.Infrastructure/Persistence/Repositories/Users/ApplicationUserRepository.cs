using FeatureBasedFolderStructure.Domain.Entities.Users;
using FeatureBasedFolderStructure.Domain.Enums;
using FeatureBasedFolderStructure.Domain.Interfaces;
using FeatureBasedFolderStructure.Domain.Interfaces.Users;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FeatureBasedFolderStructure.Infrastructure.Persistence.Repositories;

public class ApplicationUserRepository(ApplicationDbContext context) : BaseRepository<ApplicationUser, Guid>(context), IApplicationUserRepository
{
    public async Task<ApplicationUser?> GetUserWithRolesAndClaims(Guid userId)
    {
        return await AsQueryable()
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r.RoleClaims)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await AsQueryable().FirstOrDefaultAsync(e => e.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<ApplicationUser>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default)
    {
        return await AsQueryable().Where(e => e.Status == status).ToListAsync(cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await AsQueryable().AnyAsync(e => e.Email == email, cancellationToken);
    }
}