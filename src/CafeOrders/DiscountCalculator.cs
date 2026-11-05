namespace CafeOrders;

public class DiscountCalculator
{
    // R12: Replace Primitive with Value Obj (PromoCode)
    public decimal CalculatePromoDiscount(PromoCode promoCode, decimal subtotal)
    {
        if (promoCode.IsEmpty)
        {
            return 0m;
        }

        string code = promoCode.Value.ToUpperInvariant();
        if (code == "GOLD2026")
        {
            return subtotal * 0.15m;
        }
        else if (code == "SILVER2026")
        {
            return subtotal * 0.10m;
        }
        else if (code == "WELCOME")
        {
            return subtotal >= 100m ? 20m : 0m;
        }

        // Невідомий промокод мовчки повертає 0
        return 0m;
    }

    // R08: Replace Conditional with Table (Tiers)
    private static readonly (int MinPoints, string Tier)[] LoyaltyTierTable =
    {
        (1000, "Platinum"),
        (500, "Gold"),
        (200, "Silver"),
        (50, "Bronze")
    };

    public string GetLoyaltyTier(int points)
    {
        foreach (var (minPoints, tier) in LoyaltyTierTable)
        {
            if (points >= minPoints)
            {
                return tier;
            }
        }
        return "Standard";
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
