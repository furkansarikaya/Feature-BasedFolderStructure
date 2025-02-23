using FeatureBasedFolderStructure.Domain.Common;

namespace FeatureBasedFolderStructure.Domain.Entities;

public class Category : BaseAuditableEntity<int>
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public List<Product> Products { get; set; } = [];
}