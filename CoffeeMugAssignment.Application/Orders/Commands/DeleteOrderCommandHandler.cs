using MediatR;
using Microsoft.EntityFrameworkCore;
using CoffeeMugAssignment.Application.Abstractions;
using CoffeeMugAssignment.Application.Common.Exceptions;

namespace CoffeeMugAssignment.Application.Orders.Commands;

public sealed class DeleteOrderCommandHandler(IInventoryDbContext dbContext)
    : IRequestHandler<DeleteOrderCommand>
{
    public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders
            .Include(order => order.Items)
            .FirstOrDefaultAsync(order => order.Id == request.Id, cancellationToken)
            ?? throw new EntityNotFoundException("Order", request.Id);

        // Cancelling an order releases the stock it had reserved.
        var productIds = order.Items.Select(item => item.ProductId).ToArray();
        var products = await dbContext.Products
            .Where(product => productIds.Contains(product.Id))
            .ToDictionaryAsync(product => product.Id, cancellationToken);

        foreach (var item in order.Items)
        {
            if (products.TryGetValue(item.ProductId, out var product))
            {
                product.Stock += item.Quantity;
            }
        }

        dbContext.Orders.Remove(order);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
