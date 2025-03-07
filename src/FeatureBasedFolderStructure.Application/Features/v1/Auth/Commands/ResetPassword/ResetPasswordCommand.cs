using FeatureBasedFolderStructure.Application.Common.Models;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.ResetPassword;

public class ResetPasswordCommand : IRequest<BaseResponse<string>>
{
    public string Email { get; set; } = null!;
}