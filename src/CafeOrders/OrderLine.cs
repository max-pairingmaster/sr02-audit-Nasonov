namespace CafeOrders;

public class OrderLine
{
    public Product Product { get; set; }
    public int Quantity { get; set; }

    public decimal Subtotal => Product.Price * Quantity;

    public OrderLine(Product product, int quantity)
    {
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
