using FeatureBasedFolderStructure.Application.Features.Categories.DTOs;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.Categories.Queries.GetCategories;

public class GetCategoriesQuery:IRequest<List<CategoryDto>>
{
    
}