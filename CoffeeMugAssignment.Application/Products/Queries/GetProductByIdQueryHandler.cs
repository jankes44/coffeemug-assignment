using MediatR;
using Microsoft.EntityFrameworkCore;
using CoffeeMugAssignment.Application.Abstractions;
using CoffeeMugAssignment.Application.Common.Exceptions;
using CoffeeMugAssignment.Application.Products.Models;

namespace CoffeeMugAssignment.Application.Products.Queries;

public sealed class GetProductByIdQueryHandler(IInventoryDbContext dbContext)
    : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(product => product.Id == request.Id, cancellationToken)
            ?? throw new EntityNotFoundException("Product", request.Id);

        return new ProductDto(product.Id, product.Name, product.Description, product.Price, product.Stock);
    }
}
