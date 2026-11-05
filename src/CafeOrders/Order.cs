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

    // D-02: Публічна колекція (порушення інкапсуляції)
    public List<OrderLine> Lines { get; set; } = new List<OrderLine>();

    // D-09: Тимчасове поле (_tempCalc)
    private decimal _tempCalc;

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

            Lines.Add(new OrderLine(product, quantity));
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
        _tempCalc = 0m;
        return true;
    }
}
