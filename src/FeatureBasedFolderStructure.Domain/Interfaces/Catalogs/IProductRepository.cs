using FeatureBasedFolderStructure.Domain.Common.Interfaces;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;

namespace FeatureBasedFolderStructure.Domain.Interfaces.Catalogs;

public interface IProductRepository : IRepository<Product, int>
{
    Task<IEnumerable<Product>> GetProductsByCategory(int categoryId);
    Task<Product?> GetProductWithCategory(int id);
}