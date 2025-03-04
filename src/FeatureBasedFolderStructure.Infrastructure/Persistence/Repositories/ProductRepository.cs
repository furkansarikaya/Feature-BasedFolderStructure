using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using FeatureBasedFolderStructure.Domain.Interfaces;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FeatureBasedFolderStructure.Infrastructure.Persistence.Repositories;

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