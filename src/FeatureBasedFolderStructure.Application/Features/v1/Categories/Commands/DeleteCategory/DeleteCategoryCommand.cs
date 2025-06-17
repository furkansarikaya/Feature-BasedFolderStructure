using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(int Id) : IRequest<Unit>;