using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using FS.AspNetCore.ResponseWrapper.Exceptions;
using FS.EntityFramework.Library.UnitOfWorks;
using FS.Mediator.Features.RequestHandling.Core;
using Microsoft.Extensions.Logging;

namespace FeatureBasedFolderStructure.Application.Features.v1.Products.Commands.DeleteProduct;

public sealed record DeleteProductCommand(int Id) : IRequest<Unit>;

internal class DeleteProductCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<DeleteProductCommandHandler> logger) 
    : IRequestHandler<DeleteProductCommand, Unit>
{
    public async Task<Unit> HandleAsync(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var productRepository = unitOfWork.GetRepository<Product, int>();
        var entity = await productRepository.GetByIdAsync(request.Id, cancellationToken: cancellationToken);
        
        if (entity == null)
            throw new NotFoundException(nameof(Product), request.Id);
        
        await productRepository.DeleteAsync(entity, cancellationToken: cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Deleted Product {ProductId}", request.Id);

        return Unit.Value;
    }
}