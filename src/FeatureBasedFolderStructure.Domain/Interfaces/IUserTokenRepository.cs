using FeatureBasedFolderStructure.Domain.Common.Interfaces;
using FeatureBasedFolderStructure.Domain.Entities.Users;
using FeatureBasedFolderStructure.Domain.Enums;

namespace FeatureBasedFolderStructure.Domain.Interfaces;

public interface IUserTokenRepository : IRepository<UserToken, int>
{
    Task<UserToken?> GetByTokenValueAsync(string tokenValue);
    Task<IEnumerable<UserToken>> GetExpiredTokensAsync();
    Task<UserToken?> GetByUserIdAndTypeAsync(Guid userId, TokenType tokenType);
    Task DeleteExpiredTokensAsync();
}