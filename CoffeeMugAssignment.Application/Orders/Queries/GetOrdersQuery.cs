using MediatR;
using CoffeeMugAssignment.Application.Orders.Models;

namespace CoffeeMugAssignment.Application.Orders.Queries;

public sealed record GetOrdersQuery() : IRequest<IReadOnlyCollection<OrderDto>>;
