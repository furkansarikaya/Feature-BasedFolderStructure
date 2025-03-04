using FeatureBasedFolderStructure.Application.Common.Exceptions;
using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Application.Features.Products.Rules;
using FeatureBasedFolderStructure.Domain.Entities;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using FeatureBasedFolderStructure.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FeatureBasedFolderStructure.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler(
    IProductRepository productRepository,
    ProductBusinessRules productBusinessRules,
    ILogger<CreateProductCommandHandler> logger)
    : IRequestHandler<CreateProductCommand, BaseResponse<int>>
{
    public async Task<BaseResponse<int>> Handle(CreateProductCommand request, 
        CancellationToken cancellationToken)
    {
        await productBusinessRules.CheckIfCategoryExists(request.CategoryId, cancellationToken);
        
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