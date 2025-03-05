using FeatureBasedFolderStructure.Domain.Common.Interfaces;
using FeatureBasedFolderStructure.Domain.Entities.Orders;

namespace FeatureBasedFolderStructure.Domain.Interfaces.Orders;

public interface IOrderRepository : IRepository<Order, Guid>
{
    Task<Order?> GetOrderWithItems(Guid id);
    Task<IEnumerable<Order>> GetOrdersByCustomerId(string customerId);
    Task<decimal> GetTotalOrderAmount(string customerId);
}