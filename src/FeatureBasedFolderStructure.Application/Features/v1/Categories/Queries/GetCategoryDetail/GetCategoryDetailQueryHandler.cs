using AutoMapper;
using FeatureBasedFolderStructure.Application.Common.Exceptions;
using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Application.Features.v1.Categories.DTOs;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using FeatureBasedFolderStructure.Domain.Interfaces;
using FeatureBasedFolderStructure.Domain.Interfaces.Catalogs;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Categories.Queries.GetCategoryDetail;

public class GetCategoryDetailQueryHandler(
    ICategoryRepository categoryRepository,
    IMapper mapper) : IRequestHandler<GetCategoryDetailQuery, BaseResponse<CategoryDto>>
{
    public async Task<BaseResponse<CategoryDto>> Handle(GetCategoryDetailQuery request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken, true);
        if (category == null)
            throw new NotFoundException(nameof(Category), request.Id);
        return BaseResponse<CategoryDto>.SuccessResult(mapper.Map<CategoryDto>(category));
    }
}