using FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.Login;
using FluentValidation;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Validators;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email alanı zorunludur.")
            .EmailAddress().WithMessage("Geçerli bir email adresi girilmelidir.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre alanı zorunludur.");
    }
}