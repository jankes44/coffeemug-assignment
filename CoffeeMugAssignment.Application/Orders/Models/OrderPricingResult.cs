namespace CoffeeMugAssignment.Application.Orders.Models;

public sealed record OrderPricingResult(decimal Subtotal, decimal DiscountAmount, decimal TotalAmount, IReadOnlyCollection<PricedLine> Lines);

public sealed record PricedLine(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal BaseUnitPrice,
    decimal AdjustedUnitPrice,
    decimal DiscountRate,
    decimal LineTotal,
    bool WasMostExpensive);
