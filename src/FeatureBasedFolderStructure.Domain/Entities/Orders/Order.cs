using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using FeatureBasedFolderStructure.Domain.Enums;
using FeatureBasedFolderStructure.Domain.ValueObjects.Orders;
using FS.EntityFramework.Library.Common;

namespace FeatureBasedFolderStructure.Domain.Entities.Orders;

public sealed class Order : BaseAuditableEntity<Guid>
{
    public string OrderNumber { get; private set; }
    public DateTime OrderDate { get; private set; }
    public string CustomerId { get; private set; }
    public Address ShippingAddress { get; set; } = new();
    public OrderStatus Status { get; private set; }
    public List<OrderItem> OrderItems { get; private set; } = [];
    public decimal TotalAmount { get; private set; }

    public Order()
    {
        OrderDate = DateTime.UtcNow;
        OrderNumber = GenerateOrderNumber();
        Status = OrderStatus.Pending;
    }

    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}";
    }

    public void AddOrderItem(Product product, int quantity)
    {
        var existingItem = OrderItems.FirstOrDefault(item => item.ProductId == product.Id);
        
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(quantity);
        }
        else
        {
            OrderItems.Add(new OrderItem(product, quantity));
        }

        CalculateTotalAmount();
    }

    public void UpdateStatus(OrderStatus newStatus)
    {
        Status = newStatus;
    }

    private void CalculateTotalAmount()
    {
        TotalAmount = OrderItems.Sum(item => item.TotalPrice);
    }
}