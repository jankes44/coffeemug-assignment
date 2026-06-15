using MediatR;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using CoffeeMugAssignment;
using CoffeeMugAssignment.Application;
using CoffeeMugAssignment.Application.Orders.Commands;
using CoffeeMugAssignment.Application.Orders.Queries;
using CoffeeMugAssignment.Application.Products.Commands;
using CoffeeMugAssignment.Application.Products.Queries;
using CoffeeMugAssignment.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

    // Only auto-migrate and seed in dev. In prod I'd run migrations as a deploy step.
    if (app.Environment.IsDevelopment())
    {
        await dbContext.Database.MigrateAsync();
        await InventoryDataSeeder.SeedAsync(dbContext);
    }
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/", () => Results.Ok("Inventory API"));

app.MapGet("/products", async (ISender sender, CancellationToken cancellationToken)
    => Results.Ok(await sender.Send(new GetProductsQuery(), cancellationToken)));

app.MapGet("/products/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken)
    => Results.Ok(await sender.Send(new GetProductByIdQuery(id), cancellationToken)));

app.MapPost("/products", async (CreateProductCommand command, ISender sender, CancellationToken cancellationToken) =>
{
    var product = await sender.Send(command, cancellationToken);
    return Results.Created($"/products/{product.Id}", product);
});

app.MapPut("/products/{id:guid}", async (Guid id, UpdateProductRequest request, ISender sender, CancellationToken cancellationToken) =>
{
    var product = await sender.Send(new UpdateProductCommand(id, request.Name, request.Description, request.Price, request.Stock), cancellationToken);
    return Results.Ok(product);
});

app.MapDelete("/products/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
{
    await sender.Send(new DeleteProductCommand(id), cancellationToken);
    return Results.NoContent();
});

app.MapGet("/orders", async (ISender sender, CancellationToken cancellationToken)
    => Results.Ok(await sender.Send(new GetOrdersQuery(), cancellationToken)));

app.MapGet("/orders/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken)
    => Results.Ok(await sender.Send(new GetOrderByIdQuery(id), cancellationToken)));

app.MapPost("/orders", async (CreateOrderCommand command, ISender sender, CancellationToken cancellationToken) =>
{
    var order = await sender.Send(command, cancellationToken);
    return Results.Created($"/orders/{order.Id}", order);
});

app.MapDelete("/orders/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
{
    await sender.Send(new DeleteOrderCommand(id), cancellationToken);
    return Results.NoContent();
});

app.Run();

public sealed record UpdateProductRequest(string Name, string Description, decimal Price, int Stock);

public partial class Program { }
