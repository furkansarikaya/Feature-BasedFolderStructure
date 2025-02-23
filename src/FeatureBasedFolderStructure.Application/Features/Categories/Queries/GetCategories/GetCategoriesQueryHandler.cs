using AutoMapper;
using FeatureBasedFolderStructure.Application.Features.Categories.DTOs;
using FeatureBasedFolderStructure.Domain.Interfaces;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler(ICategoryRepository categoryRepository,
    IMapper mapper) : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetAllAsync(cancellationToken);
        return mapper.Map<List<CategoryDto>>(categories.OrderBy(c => c.Name).ToList());
    }
}