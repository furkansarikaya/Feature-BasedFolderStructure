using FeatureBasedFolderStructure.Application.Common.Exceptions;
using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.Rules;
using FeatureBasedFolderStructure.Application.Interfaces.Users;
using FeatureBasedFolderStructure.Domain.Entities.Users;
using FeatureBasedFolderStructure.Domain.Enums;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler(IApplicationUserService applicationUserService, ITokenService tokenService, AuthBusinessRules authBusinessRules)
    : IRequestHandler<ForgotPasswordCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await applicationUserService.GetByEmailAsync(request.Email, cancellationToken);
        if (!user.Success)
            throw new NotFoundException(nameof(ApplicationUser), request.Email);

        var resetToken = await tokenService.GenerateTokenAsync(user.Data.Id, TokenType.ResetPassword);
        return BaseResponse<string>.SuccessResult(resetToken.Token);
    }
}