using FeatureBasedFolderStructure.Domain.Enums;
using FeatureBasedFolderStructure.Domain.ValueObjects.Catalogs;
using FS.EntityFramework.Library.Common;

namespace FeatureBasedFolderStructure.Domain.Entities.Catalogs;

public sealed class Product : BaseAuditableEntity<int>, ISoftDelete
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public ProductStatus Status { get; set; }
    public Money CurrentPrice { get; private set; } = new(0, "TRY");

    public void UpdatePrice(decimal newPrice, string currency)
    {
        CurrentPrice = new Money(newPrice, currency);
    }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}