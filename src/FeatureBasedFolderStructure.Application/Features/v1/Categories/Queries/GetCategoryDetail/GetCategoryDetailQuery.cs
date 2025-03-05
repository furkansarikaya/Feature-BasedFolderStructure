using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Application.Features.v1.Categories.DTOs;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Categories.Queries.GetCategoryDetail;

public class GetCategoryDetailQuery:IRequest<BaseResponse<CategoryDto>>
{
    public int Id { get; set; }
}