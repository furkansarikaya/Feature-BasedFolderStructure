using FeatureBasedFolderStructure.Domain.Common.Attributes;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using FeatureBasedFolderStructure.Domain.Interfaces.Catalogs;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureBasedFolderStructure.Infrastructure.Persistence.Repositories.Catalogs;

[ServiceRegistration(ServiceLifetime.Scoped, Order = 1)]
public class ProductRepository(ApplicationDbContext context) : BaseRepository<Product, int>(context), IProductRepository
{
    public async Task<IEnumerable<Product>> GetProductsByCategory(int categoryId)
    {
        return await context.Products
            .Where(p => p.CategoryId == categoryId)
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task<Product?> GetProductWithCategory(int id)
    {
        return await context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}