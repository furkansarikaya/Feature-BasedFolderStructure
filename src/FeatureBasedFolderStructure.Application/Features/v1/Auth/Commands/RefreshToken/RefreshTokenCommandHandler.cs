using System.Security.Claims;
using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.DTOs;
using FeatureBasedFolderStructure.Domain.Enums;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    ITokenService tokenService) 
    : IRequestHandler<RefreshTokenCommand, BaseResponse<RefreshTokenDto>>
{
    public async Task<BaseResponse<RefreshTokenDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = tokenService.GetPrincipalFromToken(request.AccessToken, false);
    
        if (principal == null)
            return BaseResponse<RefreshTokenDto>.ErrorResult("Yetkilendirme hatası", ["Geçersiz refresh token formatı."]);
    
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            return BaseResponse<RefreshTokenDto>.ErrorResult("Yetkilendirme hatası", ["Token'dan kullanıcı kimliği alınamadı."]);
    
        var isValid = await tokenService.ValidateTokenAsync(userId, request.RefreshToken, TokenType.RefreshToken);
    
        if (!isValid)
            return BaseResponse<RefreshTokenDto>.ErrorResult("Yetkilendirme hatası", ["Geçersiz veya süresi dolmuş refresh token."]);

        var result = await tokenService.RefreshTokenAsync(userId, request.AccessToken, request.RefreshToken);
    
        if (result == null)
            return BaseResponse<RefreshTokenDto>.ErrorResult("Yetkilendirme hatası", ["Token yenileme işlemi başarısız."]);

        var response = new RefreshTokenDto(
            result.AccessToken.Token,
            result.AccessToken.ExpiryDate,
            result.RefreshToken.Token,
            result.RefreshToken.ExpiryDate
        );
        return BaseResponse<RefreshTokenDto>.SuccessResult(response);
    }
}