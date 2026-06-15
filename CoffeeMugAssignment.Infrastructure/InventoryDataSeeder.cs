using Microsoft.EntityFrameworkCore;
using CoffeeMugAssignment.Domain.Entities;

namespace CoffeeMugAssignment.Infrastructure;

// Drops a few sample products in so the API isn't empty on first run.
// Only runs when the table is empty, so it's safe to call on every startup.
public static class InventoryDataSeeder
{
    public static async Task SeedAsync(InventoryDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Products.AnyAsync(cancellationToken))
        {
            return;
        }

        dbContext.Products.AddRange(
            new Product { Id = Guid.NewGuid(), Name = "Wireless Mouse", Description = "Ergonomic 2.4GHz wireless mouse", Price = 25.99m, Stock = 100 },
            new Product { Id = Guid.NewGuid(), Name = "Mechanical Keyboard", Description = "Hot-swappable RGB mechanical keyboard", Price = 89.50m, Stock = 60 },
            new Product { Id = Guid.NewGuid(), Name = "USB-C Hub", Description = "7-in-1 USB-C multiport adapter", Price = 39.00m, Stock = 80 },
            new Product { Id = Guid.NewGuid(), Name = "27\" Monitor", Description = "27 inch 1440p IPS display", Price = 249.99m, Stock = 25 },
            new Product { Id = Guid.NewGuid(), Name = "Laptop Stand", Description = "Aluminium adjustable laptop stand", Price = 32.49m, Stock = 120 });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
