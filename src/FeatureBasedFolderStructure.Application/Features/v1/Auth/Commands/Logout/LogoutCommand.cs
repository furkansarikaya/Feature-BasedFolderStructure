using FeatureBasedFolderStructure.Application.Common.Models;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.Logout;

public record LogoutCommand(string AccessToken,string RefreshToken) : IRequest<BaseResponse<Unit>>;