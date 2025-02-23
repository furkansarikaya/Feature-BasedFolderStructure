using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}