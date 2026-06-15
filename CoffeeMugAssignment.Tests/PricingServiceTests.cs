using CoffeeMugAssignment.Application.Abstractions;
using CoffeeMugAssignment.Application.Orders.Models;
using CoffeeMugAssignment.Application.Orders.Services;
using CoffeeMugAssignment.Domain.Entities;
using CoffeeMugAssignment.Domain.Enums;

namespace CoffeeMugAssignment.Tests;

public sealed class PricingServiceTests
{
    [Fact]
    public void Calculate_AppliesVolumeDiscountWithLocationMarkup()
    {
        var service = new OrderPricingService(new StubHolidayProvider());
        var customer = new Customer { Region = CustomerRegion.Europe };

        var result = service.Calculate(
            customer,
            [new OrderPricingItem(Guid.NewGuid(), "Widget", 100m, 5)],
            new DateOnly(2026, 6, 13));

        Assert.Equal(575m, result.Subtotal);
        Assert.Equal(57.5m, result.DiscountAmount);
        Assert.Equal(517.5m, result.TotalAmount);
        Assert.Equal(0.10m, result.Lines.Single().DiscountRate);
    }

    [Fact]
    public void Calculate_AppliesBlackFridayDiscountWhenItIsHigherThanVolume()
    {
        var service = new OrderPricingService(new BlackFridayHolidayProvider());
        var customer = new Customer { Region = CustomerRegion.US };

        var result = service.Calculate(
            customer,
            [new OrderPricingItem(Guid.NewGuid(), "Widget", 10m, 50)],
            new DateOnly(2026, 11, 27));

        Assert.Equal(500m, result.Subtotal);
        Assert.Equal(150m, result.DiscountAmount);
        Assert.Equal(350m, result.TotalAmount);
        Assert.Equal(0.30m, result.Lines.Single().DiscountRate);
    }

    [Fact]
    public void Calculate_AppliesHolidaySalesDiscountToMostExpensiveProduct()
    {
        var service = new OrderPricingService(new HolidaySalesProvider());
        var customer = new Customer { Region = CustomerRegion.US };
        var expensiveId = Guid.NewGuid();
        var cheapId = Guid.NewGuid();

        var result = service.Calculate(
            customer,
            [
                new OrderPricingItem(expensiveId, "Expensive", 100m, 1),
                new OrderPricingItem(cheapId, "Cheap", 50m, 1)
            ],
            new DateOnly(2026, 6, 8));

        Assert.Equal(150m, result.Subtotal);
        Assert.Equal(15m, result.DiscountAmount);
        Assert.Equal(135m, result.TotalAmount);
        Assert.Equal(0.15m, result.Lines.Single(line => line.ProductId == expensiveId).DiscountRate);
        Assert.Equal(0m, result.Lines.Single(line => line.ProductId == cheapId).DiscountRate);
    }

    private sealed class StubHolidayProvider : IPolishHolidayProvider
    {
        public bool IsBlackFriday(DateOnly date) => false;
        public bool IsHolidaySalesDate(DateOnly date) => false;
    }

    private sealed class BlackFridayHolidayProvider : IPolishHolidayProvider
    {
        public bool IsBlackFriday(DateOnly date) => true;
        public bool IsHolidaySalesDate(DateOnly date) => false;
    }

    private sealed class HolidaySalesProvider : IPolishHolidayProvider
    {
        public bool IsBlackFriday(DateOnly date) => false;
        public bool IsHolidaySalesDate(DateOnly date) => true;
    }
}
