using FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.RefreshToken;
using FluentValidation;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Validators;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("Access token alanı zorunludur.");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token alanı zorunludur.");
    }
}