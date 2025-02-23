using FeatureBasedFolderStructure.Domain.Common;
using FeatureBasedFolderStructure.Domain.Enums;
using FeatureBasedFolderStructure.Domain.Events;
using FeatureBasedFolderStructure.Domain.ValueObjects;

namespace FeatureBasedFolderStructure.Domain.Entities;

public class Product : BaseAuditableEntity<int>
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = new();
    public ProductStatus Status { get; set; }
    public Money CurrentPrice { get; private set; } = new();

    public void UpdatePrice(decimal newPrice, string currency)
    {
        CurrentPrice = new Money(newPrice, currency);
        AddDomainEvent(new ProductPriceUpdatedEvent(this));
    }
}