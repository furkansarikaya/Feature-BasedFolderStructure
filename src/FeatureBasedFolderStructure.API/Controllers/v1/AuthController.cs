using FeatureBasedFolderStructure.API.Controllers.Base;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.ChangePassword;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.ForgotPassword;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.Login;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.Logout;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.RefreshToken;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.Register;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.ResetPassword;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.DTOs;
using FS.AspNetCore.ResponseWrapper.Models;
using Microsoft.AspNetCore.Mvc;

namespace FeatureBasedFolderStructure.API.Controllers.v1;

[ApiVersion("1.0")]
public class AuthController : BaseController
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse<RegisterDto>))]
    public async Task<RegisterDto> Register(RegisterCommand command, CancellationToken cancellationToken)
    {
        return await Mediator.Send(command, cancellationToken);
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<LoginDto>))]
    public async Task<LoginDto> Login(LoginCommand command, CancellationToken cancellationToken)
    {
        return await Mediator.Send(command, cancellationToken);
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(LogoutCommand command, CancellationToken cancellationToken)
    {
        await Mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<RefreshTokenDto>))]
    public async Task<RefreshTokenDto> RefreshToken(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        return await Mediator.Send(command, cancellationToken);
    }

    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ChangePassword(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        await Mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<string>))]
    public async Task<string> ForgotPassword(ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        return await Mediator.Send(command, cancellationToken);
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ResetPassword(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        await Mediator.Send(command, cancellationToken);
        return NoContent();
    }
}