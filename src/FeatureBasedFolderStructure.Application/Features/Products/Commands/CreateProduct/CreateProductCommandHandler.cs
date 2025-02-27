using FeatureBasedFolderStructure.Application.Common.Exceptions;
using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Domain.Entities;
using FeatureBasedFolderStructure.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FeatureBasedFolderStructure.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    ILogger<CreateProductCommandHandler> logger)
    : IRequestHandler<CreateProductCommand, BaseResponse<int>>
{
    public async Task<BaseResponse<int>> Handle(CreateProductCommand request, 
        CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if(category == null)
            throw new NotFoundException(nameof(category), request.CategoryId);
        
        var entity = new Product
        {
            Name = request.Name,
            Price = request.Price,
            Description = request.Description,
            CategoryId = request.CategoryId
        };
        entity.UpdatePrice(request.Price, "TRY");
        
        await productRepository.AddAsync(entity, cancellationToken);
        
        logger.LogInformation("Created Product {ProductId}", entity.Id);
        
        return BaseResponse<int>.SuccessResult(entity.Id);
    }
}