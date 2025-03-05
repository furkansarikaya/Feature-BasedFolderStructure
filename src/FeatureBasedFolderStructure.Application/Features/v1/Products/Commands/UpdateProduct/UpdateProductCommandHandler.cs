using FeatureBasedFolderStructure.Application.Common.Exceptions;
using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Application.Features.v1.Products.Rules;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using FeatureBasedFolderStructure.Domain.Interfaces;
using FeatureBasedFolderStructure.Domain.Interfaces.Catalogs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FeatureBasedFolderStructure.Application.Features.v1.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler(
    IProductRepository productRepository,
    ProductBusinessRules productBusinessRules,
    ILogger<UpdateProductCommandHandler> logger)
    : IRequestHandler<UpdateProductCommand, BaseResponse<Unit>>
{
    public async Task<BaseResponse<Unit>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var entity = await productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(Product), request.Id);
        
        await productBusinessRules.CheckIfCategoryExists(request.CategoryId, cancellationToken);

        entity.Name = request.Name;
        entity.Price = request.Price;
        entity.Description = request.Description;
        entity.CategoryId = request.CategoryId;

        await productRepository.UpdateAsync(entity, cancellationToken);

        logger.LogInformation("Updated Product {ProductId}", entity.Id);

        return BaseResponse<Unit>.SuccessResult(Unit.Value);
    }
}