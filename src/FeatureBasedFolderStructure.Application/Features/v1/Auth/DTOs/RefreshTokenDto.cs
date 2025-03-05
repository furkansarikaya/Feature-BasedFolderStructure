namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.DTOs;

public record RefreshTokenDto(
    string AccessToken, 
    DateTime AccessTokenExpiryDate, 
    string RefreshToken, 
    DateTime RefreshTokenExpiryDate);