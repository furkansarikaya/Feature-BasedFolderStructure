using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Products.Commands.UpdateProduct;

public class UpdateProductCommand : IRequest<Unit>
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
}