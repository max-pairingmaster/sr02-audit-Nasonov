namespace CafeOrders;

public class Product
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }

    public Product(string name, decimal price, string category = "General")
    {
        Name = name;
        Price = price;
        Category = category;
    }

    // D-10: Коментар-виправдання (excuse comment)
    // TODO: Quick hack to sync price with legacy discount system, do not remove!
    public void ChangePrice(decimal newPrice, bool notify)
    {
        if (newPrice < 0)
        {
            throw new ArgumentException("Price cannot be negative");
        }

        Price = newPrice;
        if (notify)
        {
            Console.WriteLine($"[Product Notice] Price for {Name} changed to {Price:C}");
        }
    }
}
