using FeatureBasedFolderStructure.Domain.Common.Interfaces;
using FeatureBasedFolderStructure.Domain.Entities;

namespace FeatureBasedFolderStructure.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category, int>
{
    Task<Category?> GetCategoryWithProducts(int id);
    Task<IEnumerable<Category>> GetAllWithProducts();
}