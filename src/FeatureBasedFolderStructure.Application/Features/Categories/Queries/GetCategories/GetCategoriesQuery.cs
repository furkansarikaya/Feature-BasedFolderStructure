using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Application.Features.Categories.DTOs;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.Categories.Queries.GetCategories;

public class GetCategoriesQuery:IRequest<BaseResponse<List<CategoryDto>>>
{
    
}