using FeatureBasedFolderStructure.Application.Common.Exceptions;
using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Domain.Entities;
using FeatureBasedFolderStructure.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FeatureBasedFolderStructure.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler(
    IProductRepository productRepository,
    ILogger<UpdateProductCommandHandler> logger)
    : IRequestHandler<UpdateProductCommand, BaseResponse<bool>>
{
    public async Task<BaseResponse<bool>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var entity = await productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(Product), request.Id);

        entity.Name = request.Name;
        entity.Price = request.Price;
        entity.Description = request.Description;
        entity.CategoryId = request.CategoryId;

        await productRepository.UpdateAsync(entity, cancellationToken);

        logger.LogInformation("Updated Product {ProductId}", entity.Id);

        return BaseResponse<bool>.SuccessResult(true);
    }
}