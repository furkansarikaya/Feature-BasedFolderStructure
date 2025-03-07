using FeatureBasedFolderStructure.Application.Common.Exceptions;
using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Application.Interfaces.Users;
using FeatureBasedFolderStructure.Domain.Entities.Users;
using FeatureBasedFolderStructure.Domain.Enums;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler(IApplicationUserService applicationUserService, ITokenService tokenService)
    : IRequestHandler<ResetPasswordCommand, BaseResponse<Unit>>
{
    public async Task<BaseResponse<Unit>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var validateToken = await tokenService.ValidateTokenAsync(Guid.Parse(request.UserId), request.ResetPasswordToken, TokenType.ResetPassword);
        if (!validateToken)
            return BaseResponse<Unit>.ErrorResult("Token hatası", ["Geçersiz veya süresi dolmuş token."]);

        var user = await applicationUserService.GetByIdAsync(Guid.Parse(request.UserId), cancellationToken);
        if (!user.Success)
            throw new NotFoundException(nameof(ApplicationUser), request.UserId);

        var result = await applicationUserService.ChangePasswordByForgetPasswordAsync(Guid.Parse(request.UserId), request.NewPassword, cancellationToken);
        return !result.Success ? BaseResponse<Unit>.ErrorResult(result.Message, result.Errors) : BaseResponse<Unit>.SuccessResult(Unit.Value);
    }
}