namespace CafeOrders;

public class DiscountCalculator
{
    // D-06: Одержимість примітивами (код - string)
    public decimal CalculatePromoDiscount(string promoCode, decimal subtotal)
    {
        if (string.IsNullOrWhiteSpace(promoCode))
        {
            return 0m;
        }

        // Chained string parsing
        if (promoCode.Equals("GOLD2026", StringComparison.OrdinalIgnoreCase))
        {
            return subtotal * 0.15m;
        }
        else if (promoCode.Equals("SILVER2026", StringComparison.OrdinalIgnoreCase))
        {
            return subtotal * 0.10m;
        }
        else if (promoCode.Equals("WELCOME", StringComparison.OrdinalIgnoreCase))
        {
            return subtotal >= 100m ? 20m : 0m;
        }

        // Невідомий промокод мовчки повертає 0
        return 0m;
    }

    // D-12: Довгий ланцюг if-else
    public string GetLoyaltyTier(int points)
    {
        if (points >= 1000)
        {
            return "Platinum";
        }
        else if (points >= 500)
        {
            return "Gold";
        }
        else if (points >= 200)
        {
            return "Silver";
        }
        else if (points >= 50)
        {
            return "Bronze";
        }
        else
        {
            return "Standard";
        }
    }

    // CC = 9, вкладеність 4
    public decimal ApplyHolidayDiscount(decimal amount, bool isHoliday, bool hasLoyaltyCard, int customerAge)
    {
        decimal discount = 0m;

        if (isHoliday)
        {
            if (amount > 200m)
            {
                if (hasLoyaltyCard)
                {
                    discount = amount * 0.12m;
                }
                else
                {
                    discount = amount * 0.05m;
                }
            }
            else
            {
                if (customerAge >= 60)
                {
                    discount = amount * 0.08m;
                }
            }
        }
        else
        {
            if (customerAge >= 65)
            {
                discount = amount * 0.05m;
            }
        }

        return discount;
    }
}
