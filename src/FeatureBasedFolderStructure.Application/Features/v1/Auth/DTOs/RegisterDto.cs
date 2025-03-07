namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.DTOs;

public record RegisterDto(string AccessToken,DateTime AccessTokenExpiry, string RefreshToken,DateTime RefreshTokenExpiry);