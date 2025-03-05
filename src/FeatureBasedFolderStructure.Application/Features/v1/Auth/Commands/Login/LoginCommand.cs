using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.DTOs;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<BaseResponse<LoginDto>>;