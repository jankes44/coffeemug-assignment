namespace CoffeeMugAssignment.Application.Common.Exceptions;

public sealed class InsufficientStockException(Guid productId, int requested, int available)
    : Exception($"Insufficient stock for product '{productId}'. Requested {requested}, available {available}.")
{
    public Guid ProductId { get; } = productId;

    public int Requested { get; } = requested;

    public int Available { get; } = available;
}
