using FS.EntityFramework.Library.Common;

namespace FeatureBasedFolderStructure.Domain.Entities.Catalogs;

public sealed class Category : BaseAuditableEntity<int>
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public List<Product> Products { get; set; } = [];
}