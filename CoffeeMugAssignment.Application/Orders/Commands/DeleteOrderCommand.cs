using MediatR;

namespace CoffeeMugAssignment.Application.Orders.Commands;

public sealed record DeleteOrderCommand(Guid Id) : IRequest;
