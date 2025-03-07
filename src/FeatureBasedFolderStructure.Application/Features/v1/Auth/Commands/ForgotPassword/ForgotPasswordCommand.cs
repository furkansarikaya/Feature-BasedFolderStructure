using FeatureBasedFolderStructure.Application.Common.Models;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommand : IRequest<BaseResponse<string>>
{
    public string Email { get; set; } = null!;
}