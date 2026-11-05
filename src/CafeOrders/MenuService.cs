using System.Text;

namespace CafeOrders;

public class MenuService
{
    public const decimal ServiceFeeRate = 0.10m;
    public const decimal HappyHourDiscountRate = 0.15m;
    public const int ServiceFeeItemThreshold = 5;

    private readonly DiscountCalculator _discountCalculator;
    private readonly DatabaseDummy _database;

    public MenuService(DiscountCalculator? discountCalculator = null, DatabaseDummy? database = null)
    {
        _discountCalculator = discountCalculator ?? new DiscountCalculator();
        _database = database ?? new DatabaseDummy();
    }

    // D-01: Feature Envy
    // D-03: Довгий метод (LOC ~ 65, CC = 14, глибина вкладеності 5)
    // D-04: Магічні числа (0.10m, 0.15m)
    // D-12: Довгий ланцюг if-else
    // R06: Replace Nested Conditional with Guard Clauses
    public decimal Total(Order order, string promoCode = "", DateTimeOffset? now = null)
    {
        if (order == null || order.Lines == null || order.Lines.Count == 0)
        {
            return 0m;
        }

        // R10: Move Method - розрахунок суми винесено в Order.CalculateSubtotal()
        decimal sum = order.CalculateSubtotal();

        // R01: Extract Method (ApplyHappyHour)
        var currentTime = now ?? DateTimeOffset.Now;
        sum -= ApplyHappyHour(sum, currentTime);

        // Service fee
        sum += CalculateServiceFee(order, sum);

        if (!string.IsNullOrEmpty(promoCode))
        {
            decimal promoDiscount = _discountCalculator.CalculatePromoDiscount(promoCode, sum);
            sum -= promoDiscount;
            if (sum < 0)
            {
                sum = 0;
            }
        }

        return sum;
    }

    // R01: Extract Method (ApplyHappyHour)
    private decimal ApplyHappyHour(decimal amount, DateTimeOffset currentTime)
    {
        var hour = currentTime.Hour;
        if (hour >= 14 && hour < 16)
        {
            return amount * HappyHourDiscountRate;
        }
        return 0m;
    }

    private decimal CalculateServiceFee(Order order, decimal amount)
    {
        if (order.Lines.Count > ServiceFeeItemThreshold)
        {
            return amount * ServiceFeeRate;
        }
        return 0m;
    }

    // R06: Replace Nested Conditional with Guard Clauses
    public bool ValidateOrder(Order order)
    {
        if (order == null || order.Lines == null || order.Lines.Count == 0)
        {
            return false;
        }

        if (order.Status == OrderStatus.Cancelled)
        {
            return false;
        }

        foreach (var line in order.Lines)
        {
            if (line.Quantity <= 0 || line.Product == null || line.Product.Price < 0)
            {
                return false;
            }
        }

        return true;
    }

    // D-05: Дублювання форматування
    public string PrintReceipt(Order order, string promoCode = "")
    {
        var sb = new StringBuilder();
        sb.AppendLine("========================================");
        sb.AppendLine("             CAFE ORDERS RECEIPT        ");
        sb.AppendLine("========================================");
        sb.AppendLine($"Order ID: {order.Id}");
        sb.AppendLine($"Date: {order.CreatedAt:yyyy-MM-dd HH:mm}");
        sb.AppendLine("----------------------------------------");

        foreach (var line in order.Lines)
        {
            // Дублювання форматування виводу рядків
            string lineFormatted = string.Format("{0,-20} {1,3} x {2,8:F2} = {3,8:F2}",
                line.Product.Name, line.Quantity, line.Product.Price, line.Subtotal);
            sb.AppendLine(lineFormatted);
        }

        sb.AppendLine("----------------------------------------");
        decimal total = Total(order, promoCode);
        decimal tax = CalculateTax(total);

        // Повторення форматування сум
        sb.AppendLine(string.Format("{0,-25} {1,14:F2}", "Tax (7%):", tax));
        sb.AppendLine(string.Format("{0,-25} {1,14:F2}", "TOTAL TO PAY:", total + tax));
        sb.AppendLine("========================================");

        return sb.ToString();
    }

    public decimal CalculateTax(decimal amount)
    {
        // 7% tax rate
        return decimal.Round(amount * 0.07m, 2);
    }
}
