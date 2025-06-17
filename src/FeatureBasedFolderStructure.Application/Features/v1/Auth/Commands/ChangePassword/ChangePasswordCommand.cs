using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.ChangePassword;

public class ChangePasswordCommand : IRequest<Unit>
{
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}