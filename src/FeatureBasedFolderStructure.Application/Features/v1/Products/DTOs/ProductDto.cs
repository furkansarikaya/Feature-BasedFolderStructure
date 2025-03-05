namespace FeatureBasedFolderStructure.Application.Features.v1.Products.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}