namespace CafeOrders;

public enum OrderStatus
{
    New,
    InProgress,
    Completed,
    Cancelled
}

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.New;

    // R13: Encapsulate Collection (Order.Lines)
    private readonly List<OrderLine> _lines = new();
    public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();

    public void AddLine(OrderLine line)
    {
        // R18: Introduce Guard (ThrowIfNull)
        ArgumentNullException.ThrowIfNull(line);
        _lines.Add(line);
    }

    // R10: Move Method (сумування рядків у Order)
    public decimal CalculateSubtotal() => _lines.Sum(l => l.Subtotal);

    public Order()
    {
    }

    public void AddItem(Product product, int quantity)
    {
        // D-07: Проковтнутий виняток (catch порожній)
        try
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be positive", nameof(quantity));
            }

            _lines.Add(new OrderLine(product, quantity));
        }
        catch (Exception)
        {
            // Swallowing exception deliberately in legacy code
        }
    }

    public bool Cancel(string reason)
    {
        if (Status == OrderStatus.Completed)
        {
            return false;
        }

        if (Status == OrderStatus.Cancelled)
        {
            return false;
        }

        Status = OrderStatus.Cancelled;
        return true;
    }
}
