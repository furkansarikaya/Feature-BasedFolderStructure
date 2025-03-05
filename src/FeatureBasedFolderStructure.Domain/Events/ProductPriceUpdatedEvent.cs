using FeatureBasedFolderStructure.Domain.Common;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;

namespace FeatureBasedFolderStructure.Domain.Events;

public class ProductPriceUpdatedEvent(Product product) : DomainEvent
{
    public Product Product { get; } = product;
}