namespace CafeOrders;

public class OrderLine
{
    public Product Product { get; set; }
    public int Quantity { get; set; }

    public decimal Subtotal => Product.Price * Quantity;

    public OrderLine(Product product, int quantity)
    {
        // R18: Introduce Guard (ThrowIfNull)
        ArgumentNullException.ThrowIfNull(product);
        if (quantity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity cannot be negative");
        }

        Product = product;
        Quantity = quantity;
    }

    public void UpdateQuantity(int delta)
    {
        if (Quantity + delta <= 0)
        {
            Quantity = 0;
        }
        else
        {
            Quantity += delta;
        }
    }
}
