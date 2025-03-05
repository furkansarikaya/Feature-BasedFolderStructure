namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.DTOs;

public record LoginDto(string AccessToken,DateTime AccessTokenExpiry, string RefreshToken,DateTime RefreshTokenExpiry);