using System.Net;
using FeatureBasedFolderStructure.Application.Common.Exceptions;
using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Domain.Entities;
using FeatureBasedFolderStructure.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FeatureBasedFolderStructure.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    ILogger<UpdateCategoryCommandHandler> logger)
    : IRequestHandler<UpdateCategoryCommand, BaseResponse<Unit>>
{
    public async Task<BaseResponse<Unit>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(Category), request.Id);

        entity.Name = request.Name;
        entity.Description = request.Description;

        await categoryRepository.UpdateAsync(entity, cancellationToken);

        logger.LogInformation("Updated Category {CategoryId}", entity.Id);

        return BaseResponse<Unit>.SuccessResult(Unit.Value, HttpStatusCode.NoContent);
    }
}