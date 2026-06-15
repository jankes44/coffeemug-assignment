using MediatR;
using Microsoft.EntityFrameworkCore;
using CoffeeMugAssignment.Application.Abstractions;
using CoffeeMugAssignment.Application.Products.Models;

namespace CoffeeMugAssignment.Application.Products.Queries;

public sealed class GetProductsQueryHandler(IInventoryDbContext dbContext)
    : IRequestHandler<GetProductsQuery, IReadOnlyCollection<ProductDto>>
{
    public async Task<IReadOnlyCollection<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.Products
            .AsNoTracking()
            .OrderBy(product => product.Name)
            .Select(product => new ProductDto(product.Id, product.Name, product.Description, product.Price, product.Stock))
            .ToListAsync(cancellationToken);
    }
}
