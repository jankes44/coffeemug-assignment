using MediatR;
using CoffeeMugAssignment.Application.Products.Models;

namespace CoffeeMugAssignment.Application.Products.Queries;

public sealed record GetProductsQuery() : IRequest<IReadOnlyCollection<ProductDto>>;
