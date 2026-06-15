using MediatR;
using Microsoft.EntityFrameworkCore;
using CoffeeMugAssignment.Application.Abstractions;
using CoffeeMugAssignment.Application.Common.Exceptions;
using CoffeeMugAssignment.Application.Orders.Models;

namespace CoffeeMugAssignment.Application.Orders.Queries;

public sealed class GetOrderByIdQueryHandler(IInventoryDbContext dbContext)
    : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders
            .AsNoTracking()
            .Where(order => order.Id == request.Id)
            .Select(order => new OrderDto(
                order.Id,
                order.CustomerId,
                order.Customer.Region,
                order.Subtotal,
                order.DiscountAmount,
                order.TotalAmount,
                order.Items.Select(item => new OrderItemDto(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice, item.DiscountRate, item.LineTotal)).ToArray()))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException("Order", request.Id);

        return order;
    }
}
