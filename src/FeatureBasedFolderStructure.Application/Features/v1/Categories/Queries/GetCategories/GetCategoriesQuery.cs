using System.Security.Claims;
using FeatureBasedFolderStructure.Application.Common.Attributes;
using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Application.Features.v1.Categories.DTOs;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Categories.Queries.GetCategories;

[RequiresClaim("Permission", false,"Admin","Categories.List")]
[RequiresClaim(ClaimTypes.Role, false,"Admin")]
public class GetCategoriesQuery:IRequest<BaseResponse<List<CategoryDto>>>
{
    
}