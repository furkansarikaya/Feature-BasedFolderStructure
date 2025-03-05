using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.DTOs;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    ITokenService tokenService, 
    ICurrentUserService currentUserService) 
    : IRequestHandler<RefreshTokenCommand, BaseResponse<RefreshTokenDto>>
{
    public async Task<BaseResponse<RefreshTokenDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.UserId;
        
        if (!Guid.TryParse(currentUserId, out var userId))
            return BaseResponse<RefreshTokenDto>.ErrorResult("Kimlik doğrulama hatası", ["Kullanıcı kimliği doğrulanamadı."]);

        var result = await tokenService.RefreshTokenAsync(userId, request.RefreshToken);
        
        if (result == null)
            return BaseResponse<RefreshTokenDto>.ErrorResult("Yetkilendirme hatası", ["Geçersiz veya süresi dolmuş refresh token."]);

        var (accessToken, refreshToken, accessTokenExpiryDate, refreshTokenExpiryDate) = result.Value;
        
        var response = new RefreshTokenDto(
            accessToken,
            refreshToken,
            accessTokenExpiryDate,
            refreshTokenExpiryDate
        );
        
        return BaseResponse<RefreshTokenDto>.SuccessResult(response);
    }
}