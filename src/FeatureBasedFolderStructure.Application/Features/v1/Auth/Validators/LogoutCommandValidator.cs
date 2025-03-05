using FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.Logout;
using FluentValidation;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Validators;

public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("Access token alanı zorunludur.");
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token alanı zorunludur.");
    }
}