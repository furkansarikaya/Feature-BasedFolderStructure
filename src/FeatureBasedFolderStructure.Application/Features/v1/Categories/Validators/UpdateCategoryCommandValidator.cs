using FeatureBasedFolderStructure.Application.Features.v1.Categories.Commands.UpdateCategory;
using FluentValidation;

namespace FeatureBasedFolderStructure.Application.Features.v1.Categories.Validators;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty();

        RuleFor(v => v.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(v => v.Description)
            .MaximumLength(1000);
    }
}