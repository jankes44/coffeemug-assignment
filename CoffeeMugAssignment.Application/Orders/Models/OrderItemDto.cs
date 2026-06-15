namespace CoffeeMugAssignment.Application.Orders.Models;

public sealed record OrderItemDto(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice, decimal DiscountRate, decimal LineTotal);
