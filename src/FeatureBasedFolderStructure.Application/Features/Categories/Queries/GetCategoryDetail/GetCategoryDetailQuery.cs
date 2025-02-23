using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Application.Features.Categories.DTOs;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.Categories.Queries.GetCategoryDetail;

public class GetCategoryDetailQuery:IRequest<BaseResponse<CategoryDto>>
{
    public int Id { get; set; }
}