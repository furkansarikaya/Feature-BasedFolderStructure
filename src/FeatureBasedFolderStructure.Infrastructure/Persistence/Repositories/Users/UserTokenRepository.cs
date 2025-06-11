using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Domain.Common.Attributes;
using FeatureBasedFolderStructure.Domain.Entities.Users;
using FeatureBasedFolderStructure.Domain.Enums;
using FeatureBasedFolderStructure.Domain.Interfaces.Users;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureBasedFolderStructure.Infrastructure.Persistence.Repositories.Users;

[ServiceRegistration(ServiceLifetime.Scoped, Order = 1)]
public class UserTokenRepository(ApplicationDbContext context,IDateTime dateTime) : BaseRepository<UserToken, int>(context), IUserTokenRepository
{
    public async Task<UserToken?> GetByTokenValueAsync(string tokenValue)
    {
        return await AsQueryable()
            .FirstOrDefaultAsync(t => t.TokenValue == tokenValue);
    }
    
    public async Task<IEnumerable<UserToken>> GetExpiredTokensAsync()
    {
        return await AsQueryable()
            .Where(t => t.ExpiryDate != null && t.ExpiryDate < dateTime.Now)
            .ToListAsync();
    }

    public async Task<UserToken?> GetByUserIdAndTypeAsync(Guid userId, TokenType tokenType)
    {
        return await AsQueryable()
            .FirstOrDefaultAsync(t => t.UserId == userId && t.TokenType == tokenType);
    }

    public async Task DeleteExpiredTokensAsync()
    {
        var expiredTokens = await GetExpiredTokensAsync();
        await DeleteRangeAsync(expiredTokens, CancellationToken.None, false);
    }
}