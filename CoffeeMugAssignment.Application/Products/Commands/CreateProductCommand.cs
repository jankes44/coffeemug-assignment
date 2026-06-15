using MediatR;
using CoffeeMugAssignment.Application.Products.Models;

namespace CoffeeMugAssignment.Application.Products.Commands;

public sealed record CreateProductCommand(string Name, string Description, decimal Price, int Stock) : IRequest<ProductDto>;
