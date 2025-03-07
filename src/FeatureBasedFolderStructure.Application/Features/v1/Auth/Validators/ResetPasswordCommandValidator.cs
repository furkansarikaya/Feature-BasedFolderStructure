using FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.ResetPassword;
using FluentValidation;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Validators;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email alanı zorunludur.")
            .EmailAddress().WithMessage("Geçerli bir email adresi girilmelidir.");
    }
}