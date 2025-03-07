using FeatureBasedFolderStructure.Application.Common.Models;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.ResetPassword;

public class ResetPasswordCommand : IRequest<BaseResponse<Unit>>
{
    public string UserId { get; set; } = null!;
    public string ResetPasswordToken { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}