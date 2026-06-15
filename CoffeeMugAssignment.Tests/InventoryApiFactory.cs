using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using CoffeeMugAssignment.Application.Abstractions;
using CoffeeMugAssignment.Infrastructure;

namespace CoffeeMugAssignment.Tests;

public sealed class InventoryApiFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;
    private readonly IDateTimeProvider _dateTimeProvider;

    public InventoryApiFactory(string connectionString, DateTimeOffset utcNow)
    {
        _connectionString = connectionString;
        _dateTimeProvider = new FixedDateTimeProvider(utcNow);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Not Development, so the app's startup migrate/seed steps are skipped. The
        // schema is created by the fixture; here we just point the app at the container.
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<InventoryDbContext>>();
            services.RemoveAll<InventoryDbContext>();
            services.RemoveAll<IInventoryDbContext>();
            services.RemoveAll<IDateTimeProvider>();

            services.AddSingleton(_dateTimeProvider);
            services.AddDbContext<InventoryDbContext>(options => options.UseNpgsql(_connectionString));
            services.AddScoped<IInventoryDbContext>(sp => sp.GetRequiredService<InventoryDbContext>());
        });
    }

    private sealed class FixedDateTimeProvider(DateTimeOffset utcNow) : IDateTimeProvider
    {
        public DateTimeOffset UtcNow => utcNow;
    }
}
