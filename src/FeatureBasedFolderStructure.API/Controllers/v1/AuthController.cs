using System.Net;
using FeatureBasedFolderStructure.API.Controllers.Base;
using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.Login;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.Logout;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.RefreshToken;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FeatureBasedFolderStructure.API.Controllers.v1;

[ApiVersion("1.0")]
public class AuthController : BaseController
{
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginCommand command, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(command, cancellationToken);
        return StatusCode((int)response.StatusCode, response);
    }
    
    [HttpPost("logout")]
    public async Task<ActionResult> Logout(LogoutCommand command, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(command, cancellationToken);
        return StatusCode((int)response.StatusCode, response);
    }
    
    [HttpPost("refresh-token")]
    public async Task<ActionResult<BaseResponse<RefreshTokenDto>>> RefreshToken(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(command, cancellationToken);
        return response.Success ? Ok(response) : StatusCode((int)HttpStatusCode.Unauthorized, response);
    }
}