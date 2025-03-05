namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.DTOs;

public record RefreshTokenDto(
    string AccessToken, 
    string RefreshToken, 
    DateTime AccessTokenExpiryDate, 
    DateTime RefreshTokenExpiryDate);