using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.Interfaces.Users;
using FS.Mediator.Features.RequestHandling.Core;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.ChangePassword;

public sealed record ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest<Unit>;

internal class ChangePasswordCommandHandler(IApplicationUserService applicationUserService, ICurrentUserService currentUserService)
    : IRequestHandler<ChangePasswordCommand, Unit>
{
    public async Task<Unit> HandleAsync(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(currentUserService.UserId, out var userId))
            throw new UnauthorizedAccessException("Kullanıcı bilgisi alınamadı.");

        var user = await applicationUserService.GetByIdAsync(userId, cancellationToken);
        await applicationUserService.ChangePasswordAsync(user.Id, request.CurrentPassword, request.NewPassword, cancellationToken);
        return Unit.Value;
    }
}