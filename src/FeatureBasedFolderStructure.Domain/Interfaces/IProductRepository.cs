using FeatureBasedFolderStructure.Domain.Common.Interfaces;
using FeatureBasedFolderStructure.Domain.Entities;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;

namespace FeatureBasedFolderStructure.Domain.Interfaces;

public interface IProductRepository : IRepository<Product, int>
{
    Task<IEnumerable<Product>> GetProductsByCategory(int categoryId);
    Task<Product?> GetProductWithCategory(int id);
}