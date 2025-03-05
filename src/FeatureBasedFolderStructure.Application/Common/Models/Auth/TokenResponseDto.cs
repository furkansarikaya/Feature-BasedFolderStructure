namespace FeatureBasedFolderStructure.Application.Common.Models.Auth;

public record TokenResponseDto(string Token, DateTime ExpiryDate);