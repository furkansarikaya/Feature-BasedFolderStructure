using FeatureBasedFolderStructure.Domain.Entities.Users;
using FeatureBasedFolderStructure.Domain.Enums;

namespace FeatureBasedFolderStructure.Application.Common.Interfaces;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(Guid userId, TokenType tokenType, TimeSpan? expiryDuration = null);
    Task<bool> ValidateTokenAsync(Guid userId, string token, TokenType tokenType);
    Task<bool> RevokeTokenAsync(Guid userId, string token, TokenType tokenType);
    Task<UserToken?> GetTokenAsync(Guid userId, string token, TokenType tokenType);
    Task<bool> IsTokenExpiredAsync(UserToken token);
}