using FeatureBasedFolderStructure.Domain.Common.Attributes;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using FeatureBasedFolderStructure.Domain.Interfaces.Catalogs;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Context;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureBasedFolderStructure.Infrastructure.Persistence.Repositories.Catalogs;

[ServiceRegistration(ServiceLifetime.Scoped, Order = 1)]
public class CategoryRepository(ApplicationDbContext context) : BaseRepository<Category, int>(context), ICategoryRepository
{
    public async Task<Category?> GetCategoryWithProducts(int id)
    {
        var result = await GetWithIncludesAsync(
            predicate: c => c.Id == id,
            includes: [c => c.Products]
        );
        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<Category>> GetAllWithProducts()
    {
       return await GetWithIncludesAsync(includes: [c => c.Products]);
    }
}