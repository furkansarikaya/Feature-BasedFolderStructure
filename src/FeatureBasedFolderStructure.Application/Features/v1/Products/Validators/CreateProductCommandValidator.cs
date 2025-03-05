using FeatureBasedFolderStructure.Application.Features.v1.Products.Commands.CreateProduct;
using FluentValidation;

namespace FeatureBasedFolderStructure.Application.Features.v1.Products.Validators;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(v => v.Price)
            .GreaterThan(0);

        RuleFor(v => v.Description)
            .MaximumLength(1000);
    }
}