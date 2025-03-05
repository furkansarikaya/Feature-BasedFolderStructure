using System.Security.Claims;
using FeatureBasedFolderStructure.Application.Common.Models.Auth;
using FeatureBasedFolderStructure.Domain.Entities.Users;
using FeatureBasedFolderStructure.Domain.Enums;

namespace FeatureBasedFolderStructure.Application.Common.Interfaces;

public interface ITokenService
{
    Task<TokenResponseDto> GenerateTokenAsync(Guid userId, TokenType tokenType, TimeSpan? expiryDuration = null);
    Task<bool> ValidateTokenAsync(Guid userId, string token, TokenType tokenType);
    Task<bool> RevokeTokenAsync(Guid userId, string token, TokenType tokenType);
    Task<UserToken?> GetTokenAsync(Guid userId, string token, TokenType tokenType);
    Task<bool> IsTokenExpiredAsync(UserToken token);
    Task<RefreshTokenResponseDto?> RefreshTokenAsync(Guid userId, string accessToken, string refreshToken);
    ClaimsPrincipal? GetPrincipalFromToken(string token);
}