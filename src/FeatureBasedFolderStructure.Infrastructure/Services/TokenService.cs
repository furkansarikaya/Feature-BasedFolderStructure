using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Application.Common.Settings;
using FeatureBasedFolderStructure.Domain.Entities.Users;
using FeatureBasedFolderStructure.Domain.Enums;
using FeatureBasedFolderStructure.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FeatureBasedFolderStructure.Infrastructure.Services;

public class TokenService(IApplicationUserRepository applicationUserRepository, IUserTokenRepository userTokenRepository, IDateTime dateTime, IOptions<JwtSettings> jwtSettings) : ITokenService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public async Task<(string token, DateTime expiryDate)> GenerateTokenAsync(Guid userId, TokenType tokenType, TimeSpan? expiryDuration = null)
    {
        string tokenValue;
        DateTime expiryDate;

        if (tokenType == TokenType.AccessToken)
        {
            // JWT AccessToken oluşturma
            var expiryTime = expiryDuration ?? TimeSpan.FromHours(_jwtSettings.ExpiryInHours);
            expiryDate = dateTime.Now.Add(expiryTime);
            var applicationUser = await applicationUserRepository.GetUserWithRolesAndClaims(userId);
            tokenValue = GenerateJwtToken(applicationUser, expiryDate);
        }
        else
        {
            // Diğer token tipleri için rastgele token
            tokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            expiryDate = expiryDuration.HasValue
                ? dateTime.Now.Add(expiryDuration.Value)
                : tokenType switch
                {
                    TokenType.RefreshToken => dateTime.Now.AddDays(7),
                    TokenType.ResetPassword => dateTime.Now.AddHours(24),
                    TokenType.EmailConfirmation => dateTime.Now.AddDays(7),
                    TokenType.TwoFactorAuthentication => dateTime.Now.AddMinutes(10),
                    _ => dateTime.Now.AddDays(1)
                };
        }

        // Veritabanına kaydet
        var userToken = new UserToken
        {
            UserId = userId,
            TokenType = tokenType,
            TokenValue = tokenValue,
            ExpiryDate = expiryDate
        };

        await userTokenRepository.AddAsync(userToken, CancellationToken.None);
        return (tokenValue, expiryDate);
    }

    public async Task<bool> ValidateTokenAsync(Guid userId, string token, TokenType tokenType)
    {
        if (tokenType == TokenType.AccessToken)
        {
            // JWT token doğrulama
            var principal = GetPrincipalFromToken(token);
            if (principal == null)
                return false;

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var tokenUserId) || tokenUserId != userId)
                return false;

            // Veritabanından token kontrolü
            // Eğer token veritabanında hala geçerli değilse (kullanıcı çıkış yaptıysa veya iptal edildiyse)
            var userToken = await GetTokenAsync(userId, token, tokenType);
            if (userToken == null)
                return false;

            // Token'ın süresinin dolup dolmadığını kontrol et
            return !await IsTokenExpiredAsync(userToken);
        }

        // Diğer token tipleri için veritabanı doğrulaması
        var otherUserToken = await GetTokenAsync(userId, token, tokenType);

        if (otherUserToken == null)
            return false;

        return !await IsTokenExpiredAsync(otherUserToken);
    }

    public async Task<bool> RevokeTokenAsync(Guid userId, string token, TokenType tokenType)
    {
        var userToken = await GetTokenAsync(userId, token, tokenType);

        if (userToken == null)
            return false;

        await userTokenRepository.DeleteAsync(userToken, CancellationToken.None, false);
        return true;
    }

    public async Task<UserToken?> GetTokenAsync(Guid userId, string token, TokenType tokenType)
    {
        return await userTokenRepository.AsQueryable()
            .FirstOrDefaultAsync(t =>
                t.UserId == userId &&
                t.TokenValue == token &&
                t.TokenType == tokenType);
    }

    public Task<bool> IsTokenExpiredAsync(UserToken token)
    {
        return Task.FromResult(token.ExpiryDate.HasValue && token.ExpiryDate < dateTime.Now);
    }

    public async Task<(string accessToken, string refreshToken, DateTime accessTokenExpiryDate, DateTime refreshTokenExpiryDate)?> RefreshTokenAsync(Guid userId, string accessToken, string refreshToken)
    {
        var isValid = await ValidateTokenAsync(userId, refreshToken, TokenType.RefreshToken);
        if (!isValid)
            return null;

        await RevokeTokenAsync(userId, accessToken, TokenType.AccessToken);
        await RevokeTokenAsync(userId, refreshToken, TokenType.RefreshToken);

        var (newAccessToken, accessTokenExpiryDate) = await GenerateTokenAsync(userId, TokenType.AccessToken);

        var (newRefreshToken, refreshTokenExpiryDate) = await GenerateTokenAsync(userId, TokenType.RefreshToken);
        return (newAccessToken, newRefreshToken, accessTokenExpiryDate, refreshTokenExpiryDate);
    }

    private string GenerateJwtToken(ApplicationUser applicationUser, DateTime expiryDate)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, applicationUser.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new Claim(ClaimTypes.Email, applicationUser.Email),
            new Claim(JwtRegisteredClaimNames.GivenName, applicationUser.FullName.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, applicationUser.FullName.LastName),
            new Claim(JwtRegisteredClaimNames.Exp, expiryDate.ToFileTimeUtc().ToString()),
        };
        
        foreach (var userRole in applicationUser.UserRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
        
            // Role'e ait claim'leri ekle
            claims.AddRange(userRole.Role.RoleClaims.Select(roleClaim => new Claim(roleClaim.ClaimType.ToString(), roleClaim.ClaimValue)));
        }

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiryDate,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return null;

            return principal;
        }
        catch
        {
            return null;
        }
    }
}