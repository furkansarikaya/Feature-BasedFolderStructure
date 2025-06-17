using FeatureBasedFolderStructure.Application.Common.Exceptions;
using FeatureBasedFolderStructure.Domain.Common.UnitOfWork;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FeatureBasedFolderStructure.Application.Features.v1.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<DeleteProductCommandHandler> logger) 
    : IRequestHandler<DeleteProductCommand, Unit>
{
    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var productRepository = unitOfWork.GetRepository<Product, int>();
        var entity = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (entity == null)
            throw new NotFoundException(nameof(Product), request.Id);

        await productRepository.DeleteAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken); 
        logger.LogInformation("Deleted Product {ProductId}", request.Id);

        return Unit.Value;
    }
}