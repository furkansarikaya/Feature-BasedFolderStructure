using System.Security.Claims;
using AutoMapper;
using FeatureBasedFolderStructure.Application.Common.Attributes;
using FeatureBasedFolderStructure.Application.Features.v1.Categories.DTOs;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using FS.EntityFramework.Library.UnitOfWorks;
using FS.Mediator.Features.RequestHandling.Core;

namespace FeatureBasedFolderStructure.Application.Features.v1.Categories.Queries.GetCategories;

[RequiresClaim("Permission", false,"Admin","Categories.List")]
[RequiresClaim(ClaimTypes.Role, false,"Admin")]
public sealed record GetCategoriesQuery:IRequest<List<CategoryDto>>;

internal class GetCategoriesQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper) : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    public async Task<List<CategoryDto>> HandleAsync(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categoryRepository = unitOfWork.GetRepository<Category, int>();
        var categories = await categoryRepository.GetAllAsync(cancellationToken: cancellationToken);
        return mapper.Map<List<CategoryDto>>(categories.OrderBy(c => c.Name).ToList());
    }
}