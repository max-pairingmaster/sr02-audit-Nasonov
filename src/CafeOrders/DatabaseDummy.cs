namespace CafeOrders;

public class DatabaseDummy
{
    private readonly List<Order> _orders = new();
    private readonly List<Product> _catalog = new();

    public DatabaseDummy()
    {
        _catalog.Add(new Product("Espresso", 40.0m, "Coffee"));
        _catalog.Add(new Product("Cappuccino", 65.0m, "Coffee"));
        _catalog.Add(new Product("Croissant", 55.0m, "Bakery"));
        _catalog.Add(new Product("Cheesecake", 90.0m, "Dessert"));
        _catalog.Add(new Product("Mineral Water", 30.0m, "Beverage"));
    }

    public Product? FindProductByName(string name)
    {
        return _catalog.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public void SaveOrder(Order order)
    {
        var existing = _orders.FirstOrDefault(o => o.Id == order.Id);
        if (existing != null)
        {
            _orders.Remove(existing);
        }
        _orders.Add(order);
    }

    public Order? GetOrder(Guid id)
    {
        return _orders.FirstOrDefault(o => o.Id == id);
    }

    public IReadOnlyList<Product> GetCatalog() => _catalog.AsReadOnly();
}
