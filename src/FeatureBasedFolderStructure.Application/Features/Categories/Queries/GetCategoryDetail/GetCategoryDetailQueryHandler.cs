using AutoMapper;
using FeatureBasedFolderStructure.Application.Common.Exceptions;
using FeatureBasedFolderStructure.Application.Features.Categories.DTOs;
using FeatureBasedFolderStructure.Domain.Entities;
using FeatureBasedFolderStructure.Domain.Interfaces;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.Categories.Queries.GetCategoryDetail;

public class GetCategoryDetailQueryHandler(ICategoryRepository categoryRepository,
    IMapper mapper) : IRequestHandler<GetCategoryDetailQuery, CategoryDto>
{
    public async Task<CategoryDto> Handle(GetCategoryDetailQuery request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken, true);
        if (category == null)
            throw new NotFoundException(nameof(Category), request.Id);
        return mapper.Map<CategoryDto>(category);
    }
}