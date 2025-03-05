using FeatureBasedFolderStructure.Application.Common.Models;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(int Id) : IRequest<BaseResponse<Unit>>;