using FS.EntityFramework.Library.Common;

namespace FeatureBasedFolderStructure.Domain.Entities.Catalogs;

public sealed class Category : BaseAuditableEntity<int>, ISoftDelete
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public List<Product> Products { get; set; } = [];
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}