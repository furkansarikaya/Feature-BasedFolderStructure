using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Domain.Enums;
using FS.Mediator.Features.RequestHandling.Core;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.Logout;

public sealed record LogoutCommand(string AccessToken,string RefreshToken) : IRequest<Unit>;

internal class LogoutCommandHandler(ITokenService tokenService, ICurrentUserService currentUserService) : IRequestHandler<LogoutCommand, Unit>
{
    public async Task<Unit> HandleAsync(LogoutCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.UserId;
        _ = Guid.TryParse(currentUserId, out var userId);
        if (userId == Guid.Empty)
            throw new UnauthorizedAccessException("Kullanıcı kimliği doğrulanamadı.");
        await tokenService.RevokeTokenAsync(userId, request.AccessToken, TokenType.AccessToken);
        await tokenService.RevokeTokenAsync(userId, request.RefreshToken, TokenType.RefreshToken);
        return Unit.Value;
    }
}