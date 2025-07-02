using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.Rules;
using FeatureBasedFolderStructure.Application.Interfaces.Users;
using FeatureBasedFolderStructure.Domain.Enums;
using FS.Mediator.Features.RequestHandling.Core;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : IRequest<string>;

internal class ForgotPasswordCommandHandler(IApplicationUserService applicationUserService, ITokenService tokenService, AuthBusinessRules authBusinessRules)
    : IRequestHandler<ForgotPasswordCommand, string>
{
    public async Task<string> HandleAsync(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await applicationUserService.GetByEmailAsync(request.Email, cancellationToken);
        var resetToken = await tokenService.GenerateTokenAsync(user.Id, TokenType.ResetPassword);
        return resetToken.Token;
    }
}