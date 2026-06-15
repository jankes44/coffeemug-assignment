using Microsoft.EntityFrameworkCore;
using CoffeeMugAssignment.Domain.Entities;

namespace CoffeeMugAssignment.Application.Abstractions;

public interface IInventoryDbContext
{
    DbSet<Product> Products { get; }

    DbSet<Customer> Customers { get; }

    DbSet<Order> Orders { get; }

    DbSet<OrderItem> OrderItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
