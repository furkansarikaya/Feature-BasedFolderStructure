using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Application.Interfaces.Users;
using FeatureBasedFolderStructure.Domain.Enums;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler(IApplicationUserService applicationUserService, ITokenService tokenService)
    : IRequestHandler<ResetPasswordCommand, Unit>
{
    public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var validateToken = await tokenService.ValidateTokenAsync(Guid.Parse(request.UserId), request.ResetPasswordToken, TokenType.ResetPassword);
        if (!validateToken)
            throw new UnauthorizedAccessException("Geçersiz veya Süresi dolmuş token.");

        var user = await applicationUserService.GetByIdAsync(Guid.Parse(request.UserId), cancellationToken);
        await applicationUserService.ChangePasswordByForgetPasswordAsync(user.Id, request.NewPassword, cancellationToken);
        return Unit.Value;
    }
}