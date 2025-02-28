using FeatureBasedFolderStructure.Domain.Common.Interfaces;
using FeatureBasedFolderStructure.Domain.Entities;

namespace FeatureBasedFolderStructure.Domain.Interfaces;

public interface IProductRepository : IRepository<Product, int>
{
    Task<IEnumerable<Product>> GetProductsByCategory(int categoryId);
    Task<Product?> GetProductWithCategory(int id);
}