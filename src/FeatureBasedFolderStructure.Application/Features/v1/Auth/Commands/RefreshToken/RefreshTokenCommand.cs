using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.DTOs;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<BaseResponse<RefreshTokenDto>>;