using FeatureBasedFolderStructure.Application.Features.v1.Products.DTOs;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Products.Commands.CreateProduct;

public class CreateProductCommand : IRequest<ProductDto>
{
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
}