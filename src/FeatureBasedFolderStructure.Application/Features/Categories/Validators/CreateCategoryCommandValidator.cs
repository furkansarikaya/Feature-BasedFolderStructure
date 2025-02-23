using FeatureBasedFolderStructure.Application.Features.Categories.Commands.CreateCategory;
using FluentValidation;

namespace FeatureBasedFolderStructure.Application.Features.Categories.Validators;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(v => v.Description)
            .MaximumLength(1000);
    }
}