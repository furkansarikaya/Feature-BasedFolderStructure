using FeatureBasedFolderStructure.Application.Features.v1.Auth.DTOs;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.Register;

public class RegisterCommand : IRequest<RegisterDto>
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}