using MediatR;
using Microsoft.EntityFrameworkCore;
using CoffeeMugAssignment.Application.Abstractions;
using CoffeeMugAssignment.Application.Orders.Models;

namespace CoffeeMugAssignment.Application.Orders.Queries;

public sealed class GetOrdersQueryHandler(IInventoryDbContext dbContext)
    : IRequestHandler<GetOrdersQuery, IReadOnlyCollection<OrderDto>>
{
    public async Task<IReadOnlyCollection<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.Orders
            .AsNoTracking()
            .OrderByDescending(order => order.PlacedAt)
            .Select(order => new OrderDto(
                order.Id,
                order.CustomerId,
                order.Customer.Region,
                order.Subtotal,
                order.DiscountAmount,
                order.TotalAmount,
                order.Items.Select(item => new OrderItemDto(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice, item.DiscountRate, item.LineTotal)).ToArray()))
            .ToListAsync(cancellationToken);
    }
}
