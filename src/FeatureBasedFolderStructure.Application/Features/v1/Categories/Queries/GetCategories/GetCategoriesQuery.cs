using FeatureBasedFolderStructure.Application.Common.Attributes;
using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Application.Features.Categories.DTOs;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.Categories.Queries.GetCategories;

[RequiresClaim("Permission", false,"Categories.List")]
[RequiresClaim("Role", false,"Admin")]
public class GetCategoriesQuery:IRequest<BaseResponse<List<CategoryDto>>>
{
    
}