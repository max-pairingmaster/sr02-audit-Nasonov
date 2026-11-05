using System;
using CafeOrders;

namespace CafeOrders;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== CAFE ORDERS SYSTEM ===");

        var service = new MenuService();
        var order = new Order
        {
            Id = new Guid("11111111-2222-3333-4444-555555555555"),
            CreatedAt = new DateTime(2026, 11, 5, 12, 0, 0, DateTimeKind.Utc)
        };

        var espresso = new Product("Espresso", 40.0m, "Coffee");
        var cappuccino = new Product("Cappuccino", 65.0m, "Coffee");
        var croissant = new Product("Croissant", 55.0m, "Bakery");

        order.AddItem(espresso, 2);      // 80.0
        order.AddItem(cappuccino, 1);    // 65.0
        order.AddItem(croissant, 2);     // 110.0

        Console.WriteLine($"Order lines count: {order.Lines.Count}");
        Console.WriteLine($"Order valid: {service.ValidateOrder(order)}");

        decimal totalRegular = service.Total(order);
        Console.WriteLine($"Total (Regular): {totalRegular:F2}");

        decimal totalGold = service.Total(order, "GOLD2026");
        Console.WriteLine($"Total (GOLD2026 Promo): {totalGold:F2}");

        decimal totalSilver = service.Total(order, "SILVER2026");
        Console.WriteLine($"Total (SILVER2026 Promo): {totalSilver:F2}");

        decimal tax = service.CalculateTax(totalRegular);
        Console.WriteLine($"Tax on regular: {tax:F2}");

        Console.WriteLine("\n--- RECEIPT OUTPUT ---");
        Console.Write(service.PrintReceipt(order, "GOLD2026"));
        Console.WriteLine("=== END OF EXECUTION ===");
    }
}
