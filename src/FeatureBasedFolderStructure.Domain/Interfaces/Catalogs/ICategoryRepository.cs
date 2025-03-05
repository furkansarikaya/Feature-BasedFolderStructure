using FeatureBasedFolderStructure.Domain.Common.Interfaces;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;

namespace FeatureBasedFolderStructure.Domain.Interfaces.Catalogs;

public interface ICategoryRepository : IRepository<Category, int>
{
    Task<Category?> GetCategoryWithProducts(int id);
    Task<IEnumerable<Category>> GetAllWithProducts();
}