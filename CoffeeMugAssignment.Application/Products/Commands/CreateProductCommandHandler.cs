using MediatR;
using CoffeeMugAssignment.Application.Abstractions;
using CoffeeMugAssignment.Application.Products.Models;
using CoffeeMugAssignment.Domain.Entities;

namespace CoffeeMugAssignment.Application.Products.Commands;

public sealed class CreateProductCommandHandler(IInventoryDbContext dbContext)
    : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            Price = request.Price,
            Stock = request.Stock
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new ProductDto(product.Id, product.Name, product.Description, product.Price, product.Stock);
    }
}
