using FeatureBasedFolderStructure.Domain.Common;
using FeatureBasedFolderStructure.Domain.Entities;

namespace FeatureBasedFolderStructure.Domain.Events;

public class ProductPriceUpdatedEvent(Product product) : DomainEvent
{
    public Product Product { get; } = product;
}