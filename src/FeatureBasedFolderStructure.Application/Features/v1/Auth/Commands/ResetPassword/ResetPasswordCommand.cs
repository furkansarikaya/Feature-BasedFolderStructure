using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Application.Interfaces.Users;
using FeatureBasedFolderStructure.Domain.Enums;
using FS.Mediator.Features.RequestHandling.Core;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.ResetPassword;

public sealed record ResetPasswordCommand(string UserId, string ResetPasswordToken, string NewPassword) : IRequest<Unit>;

internal class ResetPasswordCommandHandler(IApplicationUserService applicationUserService, ITokenService tokenService)
    : IRequestHandler<ResetPasswordCommand, Unit>
{
    public async Task<Unit> HandleAsync(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var validateToken = await tokenService.ValidateTokenAsync(Guid.Parse(request.UserId), request.ResetPasswordToken, TokenType.ResetPassword);
        if (!validateToken)
            throw new UnauthorizedAccessException("Geçersiz veya Süresi dolmuş token.");

        var user = await applicationUserService.GetByIdAsync(Guid.Parse(request.UserId), cancellationToken);
        await applicationUserService.ChangePasswordByForgetPasswordAsync(user.Id, request.NewPassword, cancellationToken);
        return Unit.Value;
    }
}