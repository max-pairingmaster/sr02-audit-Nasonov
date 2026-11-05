using System;
using CafeOrders;
using Xunit;

namespace CafeOrders.Tests;

public class CharacterizationTests
{
    // 1. Характеризаційний тест з розділу 3 звіту:
    [Fact]
    public void Total_PromoCodeInvalid_ReturnsZeroDiscount()
    {
        var order = new Order();
        order.Lines.Add(new OrderLine(new Product("Coffee", 50), 2));
        var service = new MenuService();
        // Характеризаційний тест: зараз система замість викидання винятку
        // на невірний промокод мовчки не дає знижки і повертає базову суму 100.
        var actual = service.Total(order, "INVALID", new DateTimeOffset(2026, 1, 1, 10, 0, 0, TimeSpan.Zero));
        Assert.Equal(100m, actual);
    }

    // 2. Тест: пусте замовлення повертає 0
    [Fact]
    public void Total_EmptyOrder_ReturnsZero()
    {
        var order = new Order();
        var service = new MenuService();
        var actual = service.Total(order);
        Assert.Equal(0m, actual);
    }

    // 3. Тест: базовий розрахунок для простого замовлення
    [Fact]
    public void Total_SimpleOrder_ReturnsCorrectSubtotal()
    {
        var order = new Order();
        order.Lines.Add(new OrderLine(new Product("Tea", 30), 2));
        var service = new MenuService();
        var fixedTime = new DateTimeOffset(2026, 1, 1, 11, 0, 0, TimeSpan.Zero);
        var actual = service.Total(order, now: fixedTime);
        Assert.Equal(60m, actual);
    }

    // 4. Тест: Happy Hour застосовує знижку 15% (14:00 - 16:00)
    [Fact]
    public void Total_HappyHour_AppliesDiscount()
    {
        var order = new Order();
        order.Lines.Add(new OrderLine(new Product("Latte", 100), 1));
        var service = new MenuService();
        var happyHourTime = new DateTimeOffset(2026, 1, 1, 14, 30, 0, TimeSpan.Zero);
        var actual = service.Total(order, now: happyHourTime);
        Assert.Equal(85m, actual); // 100 - 15% = 85
    }

    // 5. Тест: сервісний збір 10% якщо більше 5 позицій
    [Fact]
    public void Total_MoreThanFiveItems_AppliesServiceFee()
    {
        var order = new Order();
        for (int i = 0; i < 6; i++)
        {
            order.Lines.Add(new OrderLine(new Product($"Item{i}", 10), 1));
        }
        var service = new MenuService();
        var fixedTime = new DateTimeOffset(2026, 1, 1, 10, 0, 0, TimeSpan.Zero);
        var actual = service.Total(order, now: fixedTime);
        // Сума 60 + 10% = 66
        Assert.Equal(66m, actual);
    }

    // 6. Тест: промокод GOLD2026 дає 15% знижки
    [Fact]
    public void Total_GoldLoyaltyPromo_AppliesFifteenPercentDiscount()
    {
        var order = new Order();
        order.Lines.Add(new OrderLine(new Product("Special Cake", 200), 1));
        var service = new MenuService();
        var fixedTime = new DateTimeOffset(2026, 1, 1, 10, 0, 0, TimeSpan.Zero);
        var actual = service.Total(order, "GOLD2026", fixedTime);
        // 200 - 15% = 170
        Assert.Equal(170m, actual);
    }

    // 7. Тест: промокод SILVER2026 дає 10% знижки
    [Fact]
    public void Total_SilverLoyaltyPromo_AppliesTenPercentDiscount()
    {
        var order = new Order();
        order.Lines.Add(new OrderLine(new Product("Juice", 50), 2));
        var service = new MenuService();
        var fixedTime = new DateTimeOffset(2026, 1, 1, 10, 0, 0, TimeSpan.Zero);
        var actual = service.Total(order, "SILVER2026", fixedTime);
        // 100 - 10% = 90
        Assert.Equal(90m, actual);
    }

    // 8. Тест: комбінація Happy Hour та сервісного збору
    [Fact]
    public void Total_LargeOrderWithHappyHourAndServiceFee_CalculatesCorrectly()
    {
        var order = new Order();
        for (int i = 0; i < 6; i++)
        {
            order.Lines.Add(new OrderLine(new Product($"Item{i}", 100), 1));
        }
        var service = new MenuService();
        var happyHourTime = new DateTimeOffset(2026, 1, 1, 15, 0, 0, TimeSpan.Zero);
        var actual = service.Total(order, now: happyHourTime);
        // Сума 600 -> після Happy Hour (15%): 510 -> після сервісного збору (10%): 510 * 1.10 = 561
        Assert.Equal(561m, actual);
    }

    // 9. Тест: PrintReceipt форматує чек і містить податок та підсумок
    [Fact]
    public void PrintReceipt_ContainsOrderDetailsAndTax()
    {
        var order = new Order();
        order.Lines.Add(new OrderLine(new Product("Americano", 40), 1));
        var service = new MenuService();
        string receipt = service.PrintReceipt(order);
        Assert.Contains("CAFE ORDERS RECEIPT", receipt);
        Assert.Contains("Americano", receipt);
        Assert.Contains("Tax (7%):", receipt);
        Assert.Contains("TOTAL TO PAY:", receipt);
    }

    // 10. Тест: ValidateOrder повертає false для порожнього замовлення
    [Fact]
    public void ValidateOrder_EmptyOrder_ReturnsFalse()
    {
        var order = new Order();
        var service = new MenuService();
        bool isValid = service.ValidateOrder(order);
        Assert.False(isValid);
    }

    // 11. Тест: Order Cancel змінює статус на Cancelled
    [Fact]
    public void Order_Cancel_ChangesStatus()
    {
        var order = new Order();
        bool cancelled = order.Cancel("Customer left");
        Assert.True(cancelled);
        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    // 12. Тест: DiscountCalculator святкова знижка
    [Fact]
    public void DiscountCalculator_ApplyHolidayDiscount_CalculatesCorrectly()
    {
        var calc = new DiscountCalculator();
        decimal discount = calc.ApplyHolidayDiscount(300m, isHoliday: true, hasLoyaltyCard: true, customerAge: 30);
        // 300 * 0.12 = 36
        Assert.Equal(36m, discount);
    }
}
