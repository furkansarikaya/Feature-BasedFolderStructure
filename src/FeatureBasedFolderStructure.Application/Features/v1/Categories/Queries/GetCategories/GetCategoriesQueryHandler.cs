using AutoMapper;
using FeatureBasedFolderStructure.Application.Features.v1.Categories.DTOs;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using FS.EntityFramework.Library.UnitOfWorks;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler(IUnitOfWork unitOfWork,
    IMapper mapper) : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categoryRepository = unitOfWork.GetRepository<Category, int>();
        var categories = await categoryRepository.GetAllAsync(cancellationToken: cancellationToken);
        return mapper.Map<List<CategoryDto>>(categories.OrderBy(c => c.Name).ToList());
    }
}