using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CoffeeMugAssignment.Infrastructure;

// Used by "dotnet ef" at design time. Points at Postgres so migrations target the real provider.
public sealed class InventoryDbContextFactory : IDesignTimeDbContextFactory<InventoryDbContext>
{
    public InventoryDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("INVENTORY_DB_CONNECTION")
            ?? "Host=localhost;Port=5432;Database=inventorydb;Username=inventory;Password=inventory";

        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new InventoryDbContext(options);
    }
}
