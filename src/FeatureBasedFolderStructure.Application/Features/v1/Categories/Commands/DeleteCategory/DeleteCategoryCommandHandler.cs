using FeatureBasedFolderStructure.Application.Common.Exceptions;
using FeatureBasedFolderStructure.Domain.Common.UnitOfWork;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FeatureBasedFolderStructure.Application.Features.v1.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<DeleteCategoryCommandHandler> logger) 
    : IRequestHandler<DeleteCategoryCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var categoryRepository = unitOfWork.GetRepository<Category, int>();
        var entity = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (entity == null)
            throw new NotFoundException(nameof(Category), request.Id);

        await categoryRepository.DeleteAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken); 
        logger.LogInformation("Deleted Category {CategoryId}", request.Id);

        return Unit.Value;
    }
}