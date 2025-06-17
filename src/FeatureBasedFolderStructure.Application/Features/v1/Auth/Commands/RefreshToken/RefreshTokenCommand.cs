using FeatureBasedFolderStructure.Application.Features.v1.Auth.DTOs;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string AccessToken,string RefreshToken) : IRequest<RefreshTokenDto>;