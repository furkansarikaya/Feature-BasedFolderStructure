using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Domain.Enums;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.Logout;

public class LogoutCommandHandler(ITokenService tokenService, ICurrentUserService currentUserService) : IRequestHandler<LogoutCommand, BaseResponse<Unit>>
{
    public async Task<BaseResponse<Unit>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.UserId;
        _ = Guid.TryParse(currentUserId, out var userId);
        if (userId == Guid.Empty)
            throw new UnauthorizedAccessException("Kullanıcı kimliği doğrulanamadı.");
        var resultAccessToken = await tokenService.RevokeTokenAsync(userId, request.AccessToken, TokenType.AccessToken);
        var resultRefreshToken = await tokenService.RevokeTokenAsync(userId, request.RefreshToken, TokenType.RefreshToken);
        return !resultAccessToken || !resultRefreshToken ? BaseResponse<Unit>.ErrorResult("Sistemsel hata", ["Çıkış yapılırken bir hata oluştu."]) : BaseResponse<Unit>.SuccessResult(Unit.Value);
    }
}