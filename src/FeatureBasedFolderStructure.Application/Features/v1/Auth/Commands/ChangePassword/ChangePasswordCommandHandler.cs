using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Application.Interfaces.Users;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.ChangePassword;

public class ChangePasswordCommandHandler(IApplicationUserService applicationUserService, ICurrentUserService currentUserService)
    : IRequestHandler<ChangePasswordCommand, Unit>
{
    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(currentUserService.UserId, out var userId))
            throw new UnauthorizedAccessException("Kullanıcı bilgisi alınamadı.");

        var user = await applicationUserService.GetByIdAsync(userId, cancellationToken);
        await applicationUserService.ChangePasswordAsync(user.Id, request.CurrentPassword, request.NewPassword, cancellationToken);
        return Unit.Value;
    }
}