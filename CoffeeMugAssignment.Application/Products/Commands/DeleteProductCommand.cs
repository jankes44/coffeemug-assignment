using MediatR;

namespace CoffeeMugAssignment.Application.Products.Commands;

public sealed record DeleteProductCommand(Guid Id) : IRequest;
