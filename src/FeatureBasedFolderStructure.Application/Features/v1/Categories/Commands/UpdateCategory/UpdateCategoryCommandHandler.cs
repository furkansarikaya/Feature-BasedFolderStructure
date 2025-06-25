using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using FS.AspNetCore.ResponseWrapper.Exceptions;
using FS.EntityFramework.Library.UnitOfWorks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FeatureBasedFolderStructure.Application.Features.v1.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<UpdateCategoryCommandHandler> logger)
    : IRequestHandler<UpdateCategoryCommand, Unit>
{
    public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var categoryRepository = unitOfWork.GetRepository<Category, int>();
        var entity = await categoryRepository.GetByIdAsync(request.Id, cancellationToken: cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(Category), request.Id);

        entity.Name = request.Name;
        entity.Description = request.Description;

        await categoryRepository.UpdateAsync(entity, cancellationToken: cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken); 
        logger.LogInformation("Updated Category {CategoryId}", entity.Id);

        return Unit.Value;
    }
}