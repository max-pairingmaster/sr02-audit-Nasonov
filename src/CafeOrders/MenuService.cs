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
    // Прорізано шов (Seam) через введення опціонального параметра DateTimeOffset? now = null
    public decimal Total(Order order, string promoCode = "", DateTimeOffset? now = null)
    {
        if (order != null)
        {
            if (order.Lines != null)
            {
                if (order.Lines.Count > 0)
                {
                    decimal sum = 0m;

                    // D-01: Заздрість до чужих даних (Feature Envy)
                    foreach (var line in order.Lines)
                    {
                        if (line.Product != null)
                        {
                            if (line.Quantity > 0)
                            {
                                sum += line.Product.Price * line.Quantity;
                            }
                        }
                    }

                    // Happy hour check: 14:00 - 16:00
                    var currentTime = now ?? DateTimeOffset.Now;
                    var currentHour = currentTime.Hour;
                    if (currentHour >= 14)
                    {
                        if (currentHour < 16)
                        {
                            sum -= sum * HappyHourDiscountRate;
                        }
                    }

                    // R04: Замінено 0.10 на ServiceFeeRate
                    if (order.Lines.Count > ServiceFeeItemThreshold)
                    {
                        sum += sum * ServiceFeeRate;
                    }

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
                else
                {
                    return 0m;
                }
            }
            else
            {
                return 0m;
            }
        }
        else
        {
            return 0m;
        }
    }

    // D-08: Глибока вкладеність (5 рівнів)
    public bool ValidateOrder(Order order)
    {
        if (order != null)
        {
            if (order.Lines != null)
            {
                if (order.Lines.Count > 0)
                {
                    if (order.Status != OrderStatus.Cancelled)
                    {
                        foreach (var line in order.Lines)
                        {
                            if (line.Quantity <= 0 || line.Product == null || line.Product.Price < 0)
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
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
