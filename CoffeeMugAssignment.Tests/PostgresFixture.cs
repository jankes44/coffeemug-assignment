using Microsoft.EntityFrameworkCore;
using CoffeeMugAssignment.Infrastructure;
using Testcontainers.PostgreSql;

namespace CoffeeMugAssignment.Tests;

// Spins up a single throwaway PostgreSQL container for the whole test collection and
// applies the real EF Core migrations to it, so tests run against the same engine the app ships on.
public sealed class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var dbContext = new InventoryDbContext(options);
        await dbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}

[CollectionDefinition(Name)]
public sealed class PostgresCollection : ICollectionFixture<PostgresFixture>
{
    public const string Name = "Postgres";
}
