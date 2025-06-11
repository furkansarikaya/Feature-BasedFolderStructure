using FeatureBasedFolderStructure.Domain.Common.Attributes;
using FeatureBasedFolderStructure.Domain.Entities.Orders;
using FeatureBasedFolderStructure.Domain.Interfaces.Orders;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureBasedFolderStructure.Infrastructure.Persistence.Repositories.Orders;

[ServiceRegistration(ServiceLifetime.Scoped, Order = 1)]
public class OrderRepository(ApplicationDbContext context) : BaseRepository<Order, Guid>(context), IOrderRepository
{
    public async Task<Order?> GetOrderWithItems(Guid id)
    {
        return await GetQueryable()
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Order>> GetOrdersByCustomerId(string customerId)
    {
        return await GetWithIncludesAsync(predicate: order => order.CustomerId == customerId, 
            orderBy: o => o.OrderByDescending(order => order.OrderDate),
            includes: [order => order.OrderItems]);
    }

    public async Task<decimal> GetTotalOrderAmount(string customerId)
    {
        return await GetQueryable()
            .Where(o => o.CustomerId == customerId)
            .SumAsync(o => o.TotalAmount);
    }
}