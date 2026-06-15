namespace CoffeeMugAssignment.Application.Orders.Models;

public sealed record OrderLineRequest(Guid ProductId, int Quantity);
