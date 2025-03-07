using FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.ChangePassword;
using FluentValidation;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Validators;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Şu an ki şifre alanı zorunludur.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Yeni şifre alanı zorunludur.")
            .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.")
            .Matches("[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir.")
            .Matches("[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir.")
            .Matches("[0-9]").WithMessage("Şifre en az bir rakam içermelidir.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Şifre en az bir özel karakter içermelidir.");
    }
}