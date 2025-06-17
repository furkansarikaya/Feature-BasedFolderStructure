using FeatureBasedFolderStructure.Application.Common.Exceptions;
using FeatureBasedFolderStructure.Application.Features.v1.Products.Rules;
using FeatureBasedFolderStructure.Domain.Common.UnitOfWork;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FeatureBasedFolderStructure.Application.Features.v1.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler(
    IUnitOfWork unitOfWork,
    ProductBusinessRules productBusinessRules,
    ILogger<UpdateProductCommandHandler> logger)
    : IRequestHandler<UpdateProductCommand, Unit>
{
    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var productRepository = unitOfWork.GetRepository<Product, int>();
        var entity = await productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(Product), request.Id);

        await productBusinessRules.CheckIfCategoryExists(request.CategoryId, cancellationToken);

        entity.Name = request.Name;
        entity.Price = request.Price;
        entity.Description = request.Description;
        entity.CategoryId = request.CategoryId;

        await productRepository.UpdateAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Updated Product {ProductId}", entity.Id);

        return Unit.Value;
    }
}