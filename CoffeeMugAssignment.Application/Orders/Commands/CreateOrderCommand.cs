using MediatR;
using CoffeeMugAssignment.Application.Orders.Models;

namespace CoffeeMugAssignment.Application.Orders.Commands;

public sealed record CreateOrderCommand(Guid CustomerId, IReadOnlyCollection<OrderLineRequest> Products) : IRequest<OrderDto>;
