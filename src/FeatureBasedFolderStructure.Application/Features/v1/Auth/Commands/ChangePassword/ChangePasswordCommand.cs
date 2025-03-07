using FeatureBasedFolderStructure.Application.Common.Models;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.ChangePassword;

public class ChangePasswordCommand : IRequest<BaseResponse<Unit>>
{
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}