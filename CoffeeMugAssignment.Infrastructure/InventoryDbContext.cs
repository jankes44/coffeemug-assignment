using Microsoft.EntityFrameworkCore;
using CoffeeMugAssignment.Application.Abstractions;
using CoffeeMugAssignment.Domain.Entities;
using CoffeeMugAssignment.Domain.Enums;

namespace CoffeeMugAssignment.Infrastructure;

public sealed class InventoryDbContext(DbContextOptions<InventoryDbContext> options)
    : DbContext(options), IInventoryDbContext
{
    public DbSet<Product> Products => Set<Product>();

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(builder =>
        {
            builder.HasKey(product => product.Id);
            builder.Property(product => product.Name).IsRequired().HasMaxLength(50);
            builder.Property(product => product.Description).IsRequired().HasMaxLength(50);
            builder.Property(product => product.Price).HasPrecision(18, 2);
            builder.Property(product => product.Stock).IsRequired();
        });

        modelBuilder.Entity<Customer>(builder =>
        {
            builder.HasKey(customer => customer.Id);
            builder.Property(customer => customer.Name).IsRequired().HasMaxLength(100);
            builder.Property(customer => customer.Region).IsRequired();
            builder.HasData(
                new Customer { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "US Customer", Region = CustomerRegion.US },
                new Customer { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Europe Customer", Region = CustomerRegion.Europe },
                new Customer { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Asia Customer", Region = CustomerRegion.Asia });
        });

        modelBuilder.Entity<Order>(builder =>
        {
            builder.HasKey(order => order.Id);
            builder.Property(order => order.Subtotal).HasPrecision(18, 2);
            builder.Property(order => order.DiscountAmount).HasPrecision(18, 2);
            builder.Property(order => order.TotalAmount).HasPrecision(18, 2);
            builder.HasOne(order => order.Customer).WithMany().HasForeignKey(order => order.CustomerId);
            builder.HasMany(order => order.Items).WithOne(item => item.Order).HasForeignKey(item => item.OrderId);
        });

        modelBuilder.Entity<OrderItem>(builder =>
        {
            builder.HasKey(item => item.Id);
            builder.Property(item => item.ProductName).IsRequired().HasMaxLength(50);
            builder.Property(item => item.UnitPrice).HasPrecision(18, 2);
            builder.Property(item => item.DiscountRate).HasPrecision(5, 4);
            builder.Property(item => item.LineTotal).HasPrecision(18, 2);
        });
    }
}
