using FeatureBasedFolderStructure.Application.Common.Models;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommand : IRequest<BaseResponse<bool>>
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
}