using System.Net;
using System.Net.Http.Json;
using CoffeeMugAssignment.Application.Orders.Models;
using CoffeeMugAssignment.Application.Products.Models;
using CoffeeMugAssignment.Domain.Enums;

namespace CoffeeMugAssignment.Tests;

[Collection(PostgresCollection.Name)]
public sealed class InventoryEndpointsTests
{
    private readonly PostgresFixture _fixture;

    public InventoryEndpointsTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task PostProductAndCreateOrder_DecrementsStock()
    {
        await using var factory = new InventoryApiFactory(_fixture.ConnectionString, new DateTimeOffset(2026, 6, 13, 0, 0, 0, TimeSpan.Zero));
        var client = factory.CreateClient();

        var productResponse = await client.PostAsJsonAsync("/products", new
        {
            name = "Widget",
            description = "Sample widget",
            price = 10m,
            stock = 10
        });

        productResponse.EnsureSuccessStatusCode();
        var product = await productResponse.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(product);

        var orderResponse = await client.PostAsJsonAsync("/orders", new
        {
            customerId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            products = new[]
            {
                new { productId = product!.Id, quantity = 3 }
            }
        });

        orderResponse.EnsureSuccessStatusCode();

        var products = await client.GetFromJsonAsync<List<ProductDto>>("/products");
        Assert.NotNull(products);
        Assert.Equal(7, products!.Single(item => item.Id == product.Id).Stock);
    }

    [Fact]
    public async Task PostOrder_WhenStockIsInsufficient_ReturnsConflict()
    {
        await using var factory = new InventoryApiFactory(_fixture.ConnectionString, new DateTimeOffset(2026, 6, 13, 0, 0, 0, TimeSpan.Zero));
        var client = factory.CreateClient();

        var productResponse = await client.PostAsJsonAsync("/products", new
        {
            name = "Widget",
            description = "Sample widget",
            price = 10m,
            stock = 2
        });

        var product = await productResponse.Content.ReadFromJsonAsync<ProductDto>();

        var orderResponse = await client.PostAsJsonAsync("/orders", new
        {
            customerId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            products = new[]
            {
                new { productId = product!.Id, quantity = 3 }
            }
        });

        Assert.Equal(HttpStatusCode.Conflict, orderResponse.StatusCode);
    }

    [Fact]
    public async Task PostProduct_WithInvalidPayload_ReturnsBadRequest()
    {
        await using var factory = new InventoryApiFactory(_fixture.ConnectionString, new DateTimeOffset(2026, 6, 13, 0, 0, 0, TimeSpan.Zero));
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/products", new
        {
            name = new string('a', 51),
            description = "Sample widget",
            price = 10m,
            stock = 1
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetProductById_ReturnsProduct_AndNotFoundForMissing()
    {
        await using var factory = new InventoryApiFactory(_fixture.ConnectionString, new DateTimeOffset(2026, 6, 13, 0, 0, 0, TimeSpan.Zero));
        var client = factory.CreateClient();

        var created = await client.PostAsJsonAsync("/products", new
        {
            name = "Lookup",
            description = "Lookup item",
            price = 12m,
            stock = 5
        });
        var product = await created.Content.ReadFromJsonAsync<ProductDto>();

        var found = await client.GetAsync($"/products/{product!.Id}");
        found.EnsureSuccessStatusCode();
        var fetched = await found.Content.ReadFromJsonAsync<ProductDto>();
        Assert.Equal(product.Id, fetched!.Id);

        var missing = await client.GetAsync($"/products/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_ChangesFields()
    {
        await using var factory = new InventoryApiFactory(_fixture.ConnectionString, new DateTimeOffset(2026, 6, 13, 0, 0, 0, TimeSpan.Zero));
        var client = factory.CreateClient();

        var created = await client.PostAsJsonAsync("/products", new
        {
            name = "Before",
            description = "Before desc",
            price = 10m,
            stock = 5
        });
        var product = await created.Content.ReadFromJsonAsync<ProductDto>();

        var updateResponse = await client.PutAsJsonAsync($"/products/{product!.Id}", new
        {
            name = "After",
            description = "After desc",
            price = 20m,
            stock = 8
        });
        updateResponse.EnsureSuccessStatusCode();
        var updated = await updateResponse.Content.ReadFromJsonAsync<ProductDto>();

        Assert.Equal("After", updated!.Name);
        Assert.Equal(20m, updated.Price);
        Assert.Equal(8, updated.Stock);
    }

    [Fact]
    public async Task UpdateProduct_WithInvalidPayload_ReturnsBadRequest()
    {
        await using var factory = new InventoryApiFactory(_fixture.ConnectionString, new DateTimeOffset(2026, 6, 13, 0, 0, 0, TimeSpan.Zero));
        var client = factory.CreateClient();

        var created = await client.PostAsJsonAsync("/products", new
        {
            name = "Valid",
            description = "Valid desc",
            price = 10m,
            stock = 5
        });
        var product = await created.Content.ReadFromJsonAsync<ProductDto>();

        var response = await client.PutAsJsonAsync($"/products/{product!.Id}", new
        {
            name = new string('a', 51),
            description = "desc",
            price = 10m,
            stock = 1
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_RemovesIt_AndReturnsNotFoundAfterwards()
    {
        await using var factory = new InventoryApiFactory(_fixture.ConnectionString, new DateTimeOffset(2026, 6, 13, 0, 0, 0, TimeSpan.Zero));
        var client = factory.CreateClient();

        var created = await client.PostAsJsonAsync("/products", new
        {
            name = "Disposable",
            description = "To delete",
            price = 10m,
            stock = 5
        });
        var product = await created.Content.ReadFromJsonAsync<ProductDto>();

        var deleteResponse = await client.DeleteAsync($"/products/{product!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var afterDelete = await client.GetAsync($"/products/{product.Id}");
        Assert.Equal(HttpStatusCode.NotFound, afterDelete.StatusCode);
    }

    [Fact]
    public async Task DeleteOrder_RestoresStock()
    {
        await using var factory = new InventoryApiFactory(_fixture.ConnectionString, new DateTimeOffset(2026, 6, 13, 0, 0, 0, TimeSpan.Zero));
        var client = factory.CreateClient();

        var created = await client.PostAsJsonAsync("/products", new
        {
            name = "Restockable",
            description = "Stock test",
            price = 10m,
            stock = 10
        });
        var product = await created.Content.ReadFromJsonAsync<ProductDto>();

        var orderResponse = await client.PostAsJsonAsync("/orders", new
        {
            customerId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            products = new[]
            {
                new { productId = product!.Id, quantity = 4 }
            }
        });
        orderResponse.EnsureSuccessStatusCode();
        var order = await orderResponse.Content.ReadFromJsonAsync<OrderDto>();

        var afterOrder = await client.GetFromJsonAsync<List<ProductDto>>("/products");
        Assert.Equal(6, afterOrder!.Single(item => item.Id == product.Id).Stock);

        var deleteResponse = await client.DeleteAsync($"/orders/{order!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var afterDelete = await client.GetFromJsonAsync<List<ProductDto>>("/products");
        Assert.Equal(10, afterDelete!.Single(item => item.Id == product.Id).Stock);

        var missingOrder = await client.GetAsync($"/orders/{order.Id}");
        Assert.Equal(HttpStatusCode.NotFound, missingOrder.StatusCode);
    }

    [Fact]
    public async Task GetOrders_ReturnsCreatedOrderWithItems()
    {
        await using var factory = new InventoryApiFactory(_fixture.ConnectionString, new DateTimeOffset(2026, 6, 13, 0, 0, 0, TimeSpan.Zero));
        var client = factory.CreateClient();

        var created = await client.PostAsJsonAsync("/products", new
        {
            name = "Listable",
            description = "List test",
            price = 15m,
            stock = 10
        });
        var product = await created.Content.ReadFromJsonAsync<ProductDto>();

        var orderResponse = await client.PostAsJsonAsync("/orders", new
        {
            customerId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            products = new[]
            {
                new { productId = product!.Id, quantity = 2 }
            }
        });
        orderResponse.EnsureSuccessStatusCode();
        var createdOrder = await orderResponse.Content.ReadFromJsonAsync<OrderDto>();

        var orders = await client.GetFromJsonAsync<List<OrderDto>>("/orders");
        Assert.NotNull(orders);

        var listed = orders!.Single(order => order.Id == createdOrder!.Id);
        Assert.Equal(CustomerRegion.US, listed.CustomerRegion);
        Assert.Equal(createdOrder!.TotalAmount, listed.TotalAmount);
        var line = Assert.Single(listed.Items, item => item.ProductId == product.Id);
        Assert.Equal(2, line.Quantity);
    }
}
