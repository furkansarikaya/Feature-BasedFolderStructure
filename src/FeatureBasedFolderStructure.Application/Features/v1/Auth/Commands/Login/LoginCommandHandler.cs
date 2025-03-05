using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.DTOs;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, BaseResponse<LoginDto>>
{
    public async Task<BaseResponse<LoginDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}