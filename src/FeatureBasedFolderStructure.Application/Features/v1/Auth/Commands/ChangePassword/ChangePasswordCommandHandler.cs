using FeatureBasedFolderStructure.Application.Common.Exceptions;
using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.Rules;
using FeatureBasedFolderStructure.Application.Interfaces.Users;
using FeatureBasedFolderStructure.Domain.Entities.Users;
using FeatureBasedFolderStructure.Domain.Enums;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.ChangePassword;

public class ChangePasswordCommandHandler(IApplicationUserService applicationUserService, ITokenService tokenService, ICurrentUserService currentUserService)
    : IRequestHandler<ChangePasswordCommand, BaseResponse<Unit>>
{
    public async Task<BaseResponse<Unit>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(currentUserService.UserId, out var userId))
            return BaseResponse<Unit>.ErrorResult("Kullanıcı hatası", ["Kullanıcı bilgisi alınamadı."], System.Net.HttpStatusCode.Unauthorized);

        var user = await applicationUserService.GetByIdAsync(userId, cancellationToken);
        if (!user.Success)
           throw new NotFoundException(nameof(ApplicationUser), userId);

        var result = await applicationUserService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword, cancellationToken);
        return !result.Success ? BaseResponse<Unit>.ErrorResult(result.Message, result.Errors) : BaseResponse<Unit>.SuccessResult(Unit.Value);
    }
}