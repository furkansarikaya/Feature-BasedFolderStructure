using FeatureBasedFolderStructure.Application.Common.Attributes;
using FeatureBasedFolderStructure.Application.Features.v1.Categories.DTOs;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Categories.Commands.CreateCategory;

[RequiresClaim("Permission", false, "Category.Create")]
[RequiresClaim("Role", false, "Admin")]
public class CreateCategoryCommand : IRequest<CategoryDto>
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}