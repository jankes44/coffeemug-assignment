using MediatR;
using Microsoft.EntityFrameworkCore;
using CoffeeMugAssignment.Application.Abstractions;
using CoffeeMugAssignment.Application.Common.Exceptions;
using CoffeeMugAssignment.Application.Orders.Models;
using CoffeeMugAssignment.Domain.Entities;

namespace CoffeeMugAssignment.Application.Orders.Commands;

public sealed class CreateOrderCommandHandler(
    IInventoryDbContext dbContext,
    IOrderPricingService pricingService,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<CreateOrderCommand, OrderDto>
{
    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var customer = await dbContext.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(customer => customer.Id == request.CustomerId, cancellationToken)
            ?? throw new EntityNotFoundException("Customer", request.CustomerId);

        var normalizedLines = request.Products
            .GroupBy(line => line.ProductId)
            .Select(group => new OrderLineRequest(group.Key, group.Sum(item => item.Quantity)))
            .ToArray();

        var productIds = normalizedLines.Select(line => line.ProductId).ToArray();
        var products = await dbContext.Products
            .Where(product => productIds.Contains(product.Id))
            .ToDictionaryAsync(product => product.Id, cancellationToken);

        var missingProductId = productIds.FirstOrDefault(id => !products.ContainsKey(id));
        if (missingProductId != Guid.Empty)
        {
            throw new EntityNotFoundException("Product", missingProductId);
        }

        foreach (var line in normalizedLines)
        {
            var product = products[line.ProductId];
            if (product.Stock < line.Quantity)
            {
                throw new InsufficientStockException(product.Id, line.Quantity, product.Stock);
            }
        }

        var pricingInputs = normalizedLines
            .Select(line =>
            {
                var product = products[line.ProductId];
                return new OrderPricingItem(product.Id, product.Name, product.Price, line.Quantity);
            })
            .ToArray();

        var pricing = pricingService.Calculate(customer, pricingInputs, dateTimeProvider.Today);
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            PlacedAt = dateTimeProvider.UtcNow,
            Subtotal = pricing.Subtotal,
            DiscountAmount = pricing.DiscountAmount,
            TotalAmount = pricing.TotalAmount,
            Items = pricing.Lines.Select(line => new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = line.ProductId,
                ProductName = line.ProductName,
                Quantity = line.Quantity,
                UnitPrice = line.AdjustedUnitPrice,
                DiscountRate = line.DiscountRate,
                LineTotal = line.LineTotal
            }).ToList()
        };

        foreach (var line in normalizedLines)
        {
            products[line.ProductId].Stock -= line.Quantity;
        }

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new OrderDto(
            order.Id,
            order.CustomerId,
            customer.Region,
            order.Subtotal,
            order.DiscountAmount,
            order.TotalAmount,
            order.Items.Select(item => new OrderItemDto(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice, item.DiscountRate, item.LineTotal)).ToArray());
    }
}
