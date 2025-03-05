using FeatureBasedFolderStructure.Application.Common.Models;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommand : IRequest<BaseResponse<int>>
{
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
}