using MediatR;
using Microsoft.EntityFrameworkCore;
using CoffeeMugAssignment.Application.Abstractions;
using CoffeeMugAssignment.Application.Common.Exceptions;

namespace CoffeeMugAssignment.Application.Products.Commands;

public sealed class DeleteProductCommandHandler(IInventoryDbContext dbContext)
    : IRequestHandler<DeleteProductCommand>
{
    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .FirstOrDefaultAsync(product => product.Id == request.Id, cancellationToken)
            ?? throw new EntityNotFoundException("Product", request.Id);

        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
