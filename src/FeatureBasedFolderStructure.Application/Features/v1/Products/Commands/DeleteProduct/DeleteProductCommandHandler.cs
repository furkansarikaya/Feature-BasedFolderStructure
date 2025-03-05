using FeatureBasedFolderStructure.Application.Common.Exceptions;
using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using FeatureBasedFolderStructure.Domain.Interfaces;
using FeatureBasedFolderStructure.Domain.Interfaces.Catalogs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FeatureBasedFolderStructure.Application.Features.v1.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler(
    IProductRepository productRepository,
    ILogger<DeleteProductCommandHandler> logger) 
    : IRequestHandler<DeleteProductCommand, BaseResponse<Unit>>
{
    public async Task<BaseResponse<Unit>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var entity = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (entity == null)
            throw new NotFoundException(nameof(Product), request.Id);

        await productRepository.DeleteAsync(entity, cancellationToken);
        
        logger.LogInformation("Deleted Product {ProductId}", request.Id);
        
        return BaseResponse<Unit>.SuccessResult(Unit.Value);
    }
}