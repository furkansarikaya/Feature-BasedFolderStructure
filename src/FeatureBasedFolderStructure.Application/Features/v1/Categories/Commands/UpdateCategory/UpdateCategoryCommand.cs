using FeatureBasedFolderStructure.Application.Common.Models;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommand : IRequest<BaseResponse<Unit>>
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}