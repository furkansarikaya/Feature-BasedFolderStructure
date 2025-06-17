using AutoMapper;
using FeatureBasedFolderStructure.Application.Common.Exceptions;
using FeatureBasedFolderStructure.Application.Features.v1.Categories.DTOs;
using FeatureBasedFolderStructure.Domain.Common.UnitOfWork;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Categories.Queries.GetCategoryDetail;

public class GetCategoryDetailQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper) : IRequestHandler<GetCategoryDetailQuery, CategoryDto>
{
    public async Task<CategoryDto> Handle(GetCategoryDetailQuery request, CancellationToken cancellationToken)
    {
        var categoryRepository = unitOfWork.GetRepository<Category, int>();
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken, true);
        if (category == null)
            throw new NotFoundException(nameof(Category), request.Id);
        return mapper.Map<CategoryDto>(category);
    }
}