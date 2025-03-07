using FeatureBasedFolderStructure.API.Controllers.Base;
using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.ChangePassword;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.ForgotPassword;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.Login;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.Logout;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.RefreshToken;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.Register;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FeatureBasedFolderStructure.API.Controllers.v1;

[ApiVersion("1.0")]
public class AuthController : BaseController
{
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterCommand command, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(command, cancellationToken);
        return StatusCode((int)response.StatusCode, response);
    }

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
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPost("change-password")]
    public async Task<ActionResult<BaseResponse<RefreshTokenDto>>> ChangePassword(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(command, cancellationToken);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult<BaseResponse<string>>> ResetPassword(ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(command, cancellationToken);
        return StatusCode((int)response.StatusCode, response);
    }
}