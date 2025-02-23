using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommand : IRequest<int>
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}