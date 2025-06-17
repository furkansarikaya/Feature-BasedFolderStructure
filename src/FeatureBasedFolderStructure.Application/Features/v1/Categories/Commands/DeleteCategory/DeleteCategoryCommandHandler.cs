using FeatureBasedFolderStructure.Application.Common.Exceptions;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using FeatureBasedFolderStructure.Domain.Interfaces.Catalogs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FeatureBasedFolderStructure.Application.Features.v1.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    ILogger<DeleteCategoryCommandHandler> logger) 
    : IRequestHandler<DeleteCategoryCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (entity == null)
            throw new NotFoundException(nameof(Category), request.Id);

        await categoryRepository.DeleteAsync(entity, cancellationToken);
        
        logger.LogInformation("Deleted Category {CategoryId}", request.Id);

        return Unit.Value;
    }
}