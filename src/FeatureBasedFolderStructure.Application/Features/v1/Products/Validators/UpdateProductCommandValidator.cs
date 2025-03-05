using FeatureBasedFolderStructure.Application.Features.Products.Commands.UpdateProduct;
using FluentValidation;

namespace FeatureBasedFolderStructure.Application.Features.Products.Validators;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty();

        RuleFor(v => v.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(v => v.Price)
            .GreaterThan(0);

        RuleFor(v => v.Description)
            .MaximumLength(1000);
    }
}