namespace CoffeeMugAssignment.Application.Orders.Models;

public sealed record OrderPricingItem(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity);
