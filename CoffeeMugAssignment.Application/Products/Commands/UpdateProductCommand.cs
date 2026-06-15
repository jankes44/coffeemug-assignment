using MediatR;
using CoffeeMugAssignment.Application.Products.Models;

namespace CoffeeMugAssignment.Application.Products.Commands;

public sealed record UpdateProductCommand(Guid Id, string Name, string Description, decimal Price, int Stock) : IRequest<ProductDto>;
