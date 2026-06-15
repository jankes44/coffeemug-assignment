using CoffeeMugAssignment.Domain.Enums;

namespace CoffeeMugAssignment.Application.Orders.Models;

public sealed record OrderDto(
    Guid Id,
    Guid CustomerId,
    CustomerRegion CustomerRegion,
    decimal Subtotal,
    decimal DiscountAmount,
    decimal TotalAmount,
    IReadOnlyCollection<OrderItemDto> Items);
