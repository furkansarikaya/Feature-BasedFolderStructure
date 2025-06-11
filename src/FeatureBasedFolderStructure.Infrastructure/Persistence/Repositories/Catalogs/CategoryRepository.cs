using FeatureBasedFolderStructure.Domain.Common.Attributes;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using FeatureBasedFolderStructure.Domain.Interfaces.Catalogs;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureBasedFolderStructure.Infrastructure.Persistence.Repositories.Catalogs;

[ServiceRegistration(ServiceLifetime.Scoped, Order = 1)]
public class CategoryRepository(ApplicationDbContext context) : BaseRepository<Category, int>(context), ICategoryRepository
{
    public async Task<Category?> GetCategoryWithProducts(int id)
    {
        return await AsQueryable()
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Category>> GetAllWithProducts()
    {
        return await AsQueryable()
            .Include(c => c.Products)
            .ToListAsync();
    }
}