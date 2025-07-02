using AutoMapper;
using FeatureBasedFolderStructure.Application.Features.v1.Categories.DTOs;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using FS.AspNetCore.ResponseWrapper.Exceptions;
using FS.EntityFramework.Library.UnitOfWorks;
using FS.Mediator.Features.RequestHandling.Core;

namespace FeatureBasedFolderStructure.Application.Features.v1.Categories.Queries.GetCategoryDetail;

public sealed record GetCategoryDetailQuery(int Id) : IRequest<CategoryDto>;

internal class GetCategoryDetailQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper) : IRequestHandler<GetCategoryDetailQuery, CategoryDto>
{
    public async Task<CategoryDto> HandleAsync(GetCategoryDetailQuery request, CancellationToken cancellationToken)
    {
        var categoryRepository = unitOfWork.GetRepository<Category, int>();
        var category = await categoryRepository.GetByIdAsync(request.Id, true, cancellationToken);
        if (category == null)
            throw new NotFoundException(nameof(Category), request.Id);
        return mapper.Map<CategoryDto>(category);
    }
}