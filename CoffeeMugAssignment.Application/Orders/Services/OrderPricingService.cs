using CoffeeMugAssignment.Application.Abstractions;
using CoffeeMugAssignment.Application.Orders.Models;
using CoffeeMugAssignment.Domain.Entities;
using CoffeeMugAssignment.Domain.Enums;

namespace CoffeeMugAssignment.Application.Orders.Services;

public sealed class OrderPricingService(IPolishHolidayProvider holidayProvider) : IOrderPricingService
{
    public OrderPricingResult Calculate(Customer customer, IReadOnlyCollection<OrderPricingItem> lines, DateOnly pricingDate)
    {
        if (lines.Count == 0)
        {
            return new OrderPricingResult(0, 0, 0, Array.Empty<PricedLine>());
        }

        var multiplier = customer.Region switch
        {
            CustomerRegion.US => 1.00m,
            CustomerRegion.Europe => 1.15m,
            CustomerRegion.Asia => 1.05m,
            _ => 1.00m
        };

        var holidaySale = holidayProvider.IsHolidaySalesDate(pricingDate);
        var blackFriday = holidayProvider.IsBlackFriday(pricingDate);
        var mostExpensiveProductId = holidaySale
            ? lines.OrderByDescending(line => line.UnitPrice).ThenBy(line => line.ProductName).First().ProductId
            : Guid.Empty;

        var pricedLines = lines
            .Select(line =>
            {
                var adjustedUnitPrice = RoundMoney(line.UnitPrice * multiplier);
                var volumeDiscount = GetVolumeDiscount(line.Quantity);
                var seasonalDiscount = blackFriday
                    ? 0.25m
                    : holidaySale && line.ProductId == mostExpensiveProductId
                        ? 0.15m
                        : 0m;
                var discountRate = Math.Max(volumeDiscount, seasonalDiscount);
                var lineTotal = RoundMoney(adjustedUnitPrice * line.Quantity * (1 - discountRate));

                return new PricedLine(
                    line.ProductId,
                    line.ProductName,
                    line.Quantity,
                    line.UnitPrice,
                    adjustedUnitPrice,
                    discountRate,
                    lineTotal,
                    line.ProductId == mostExpensiveProductId);
            })
            .ToArray();

        var subtotal = RoundMoney(pricedLines.Sum(line => line.AdjustedUnitPrice * line.Quantity));
        var total = RoundMoney(pricedLines.Sum(line => line.LineTotal));
        var discountAmount = RoundMoney(subtotal - total);

        return new OrderPricingResult(subtotal, discountAmount, total, pricedLines);
    }

    private static decimal GetVolumeDiscount(int quantity) => quantity switch
    {
        >= 50 => 0.30m,
        >= 10 => 0.20m,
        >= 5 => 0.10m,
        _ => 0m
    };

    private static decimal RoundMoney(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
