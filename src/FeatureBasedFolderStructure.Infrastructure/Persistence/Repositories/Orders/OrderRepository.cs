using FeatureBasedFolderStructure.Domain.Entities.Orders;
using FeatureBasedFolderStructure.Domain.Interfaces;
using FeatureBasedFolderStructure.Domain.Interfaces.Orders;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FeatureBasedFolderStructure.Infrastructure.Persistence.Repositories;

public class OrderRepository(ApplicationDbContext context) : BaseRepository<Order, Guid>(context), IOrderRepository
{
    public async Task<Order?> GetOrderWithItems(Guid id)
    {
        return await AsQueryable()
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Order>> GetOrdersByCustomerId(string customerId)
    {
        return await AsQueryable()
            .Where(o => o.CustomerId == customerId)
            .Include(o => o.OrderItems)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalOrderAmount(string customerId)
    {
        return await AsQueryable()
            .Where(o => o.CustomerId == customerId)
            .SumAsync(o => o.TotalAmount);
    }
}