using AutoMapper;
using FeatureBasedFolderStructure.Application.Features.v1.Products.DTOs;
using FeatureBasedFolderStructure.Application.Features.v1.Products.Rules;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using FS.EntityFramework.Library.UnitOfWorks;
using FS.Mediator.Features.RequestHandling.Core;
using Microsoft.Extensions.Logging;

namespace FeatureBasedFolderStructure.Application.Features.v1.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(string Name, decimal Price, string? Description, int CategoryId) : IRequest<ProductDto>;

internal class CreateProductCommandHandler(
    IUnitOfWork unitOfWork,
    ProductBusinessRules productBusinessRules,
    ILogger<CreateProductCommandHandler> logger,
    IMapper mapper)
    : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> HandleAsync(CreateProductCommand request, 
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
        
        var productRepository = unitOfWork.GetRepository<Product, int>();
        await productRepository.AddAsync(entity, cancellationToken: cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Created Product {ProductId}", entity.Id);

        return mapper.Map<ProductDto>(entity);
    }
}