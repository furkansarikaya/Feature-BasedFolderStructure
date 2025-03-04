using FeatureBasedFolderStructure.Domain.Common;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;

namespace FeatureBasedFolderStructure.Domain.Entities.Orders;

public class OrderItem : BaseEntity<Guid>
{
    public Guid OrderId { get; private set; }
    public int ProductId { get; private set; }
    public Product Product { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice { get; private set; }

    public OrderItem()
    {
        
    }

    public OrderItem(Product product, int quantity)
    {
        Product = product;
        ProductId = product.Id;
        UnitPrice = product.Price;
        UpdateQuantity(quantity);
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        Quantity = newQuantity;
        CalculateTotalPrice();
    }

    private void CalculateTotalPrice()
    {
        TotalPrice = UnitPrice * Quantity;
    }
}