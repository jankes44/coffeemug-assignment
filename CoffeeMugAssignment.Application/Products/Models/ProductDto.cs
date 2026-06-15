namespace CoffeeMugAssignment.Application.Products.Models;

public sealed record ProductDto(Guid Id, string Name, string Description, decimal Price, int Stock);
