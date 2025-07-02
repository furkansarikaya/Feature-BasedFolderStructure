using System.Security.Claims;
using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.DTOs;
using FeatureBasedFolderStructure.Domain.Enums;
using FS.AspNetCore.ResponseWrapper.Exceptions;
using FS.Mediator.Features.RequestHandling.Core;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string AccessToken,string RefreshToken) : IRequest<RefreshTokenDto>;

internal class RefreshTokenCommandHandler(
    ITokenService tokenService) 
    : IRequestHandler<RefreshTokenCommand, RefreshTokenDto>
{
    public async Task<RefreshTokenDto> HandleAsync(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = tokenService.GetPrincipalFromToken(request.AccessToken, false);
    
        if (principal == null)
            throw new UnauthorizedAccessException("Geçersiz access token formatı.");
    
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Geçersiz access token formatı: Kullanıcı kimliği alınamadı.");
    
        var isValid = await tokenService.ValidateTokenAsync(userId, request.RefreshToken, TokenType.RefreshToken);
    
        if (!isValid)
            throw new UnauthorizedAccessException("Geçersiz veya süresi dolmuş refresh token.");

        var result = await tokenService.RefreshTokenAsync(userId, request.AccessToken, request.RefreshToken);
    
        if (result == null)
            throw new BusinessException("Token yenileme işlemi başarısız");

        var response = new RefreshTokenDto(
            result.AccessToken.Token,
            result.AccessToken.ExpiryDate,
            result.RefreshToken.Token,
            result.RefreshToken.ExpiryDate
        );
        return response;
    }
}