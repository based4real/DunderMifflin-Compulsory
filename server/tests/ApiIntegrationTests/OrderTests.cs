using System.Net;
using System.Net.Http.Json;
using DataAccess;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using PgCtx;
using Service;
using Service.Models.Requests;
using Service.Models.Responses;
using SharedDependencies.Enums;
using SharedTestDependencies;

namespace ApiIntegrationTests;

public class OrderTests : WebApplicationFactory<Program>
{
    private readonly PgCtxSetup<AppDbContext> _pgCtxSetup = new();

    public OrderTests()
    {
        Environment.SetEnvironmentVariable($"{nameof(AppOptions)}:{nameof(AppOptions.LocalDbConn)}", _pgCtxSetup._postgres.GetConnectionString());
        
        SeedDatabase();
    }

    private void SeedDatabase()
    {
        var customers = TestObjects.Customers(4);
        _pgCtxSetup.DbContextInstance.Customers.AddRange(customers);
        
        var properties = TestObjects.Properties(4); 
        _pgCtxSetup.DbContextInstance.Properties.AddRange(properties);
        
        var papers = TestObjects.Papers(4, properties); 
        _pgCtxSetup.DbContextInstance.Papers.AddRange(papers);
        
        var orders = TestObjects.Orders(customers, papers);
        _pgCtxSetup.DbContextInstance.Orders.AddRange(orders);

        _pgCtxSetup.DbContextInstance.SaveChanges();
    }

    [Fact]
    public async Task CreateOrderTest()
    {
        var client = CreateClient();
        
        var createOrderModel = new OrderCreateModel
        {
            CustomerId = 1,
            OrderEntries = new List<OrderCreateEntryModel>
            {
                new()
                {
                    ProductId = 1,
                    Quantity = 2
                }
            }
        };
        
        var response = await client.PostAsJsonAsync("api/order", createOrderModel);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var responseData = await response.Content.ReadFromJsonAsync<OrderDetailViewModel>();
        
        Assert.NotNull(responseData);
        Assert.NotEqual(0, responseData.Id);
        Assert.True(responseData.TotalPrice > 0);

        Assert.NotNull(responseData.Entry);
        Assert.NotEmpty(responseData.Entry);

        var entry = createOrderModel.OrderEntries.First();
        var responseEntry = responseData.Entry.First();
        Assert.True(entry.ProductId == responseEntry.ProductId);
        Assert.True(entry.Quantity == responseEntry.Quantity);
    }
    
    [Fact]
    public async Task UpdateOrderStatus_ValidOrderIdAndStatus_ReturnsNoContent()
    {
        var client = CreateClient();
        
        var createOrderModel = new OrderCreateModel
        {
            CustomerId = 1,
            OrderEntries = []
        };

        var createOrderResponse = await client.PostAsJsonAsync("api/order", createOrderModel);
        Assert.Equal(HttpStatusCode.Created, createOrderResponse.StatusCode);

        var createdOrder = await createOrderResponse.Content.ReadFromJsonAsync<OrderDetailViewModel>();
        Assert.NotNull(createdOrder);
        var validOrderId = createdOrder.Id;

        const OrderStatus newStatus = OrderStatus.Shipped;
        var response = await client.PatchAsync($"api/order/{validOrderId}/status?status={newStatus}", null);
        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        var updatedOrder = await _pgCtxSetup.DbContextInstance.Orders.FindAsync(validOrderId);
        Assert.NotNull(updatedOrder);
        Assert.Equal(newStatus.ToString().ToLower(), updatedOrder.Status);
    }
    
    [Fact]
    public async Task UpdateOrderStatus_InvalidOrderId_ReturnsBadRequest()
    {
        // Arrange
        var client = CreateClient();
        const int invalidOrderId = -1;
        const OrderStatus newStatus = OrderStatus.Shipped;

        // Act
        var response = await client.PatchAsync($"api/order/{invalidOrderId}/status?status={newStatus}", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateOrderStatus_NonExistentOrderId_ReturnsNotFound()
    {
        // Arrange
        var client = CreateClient();
        const int nonExistentOrderId = int.MaxValue; // Antager at dette ID ikke eksister
        const OrderStatus newStatus = OrderStatus.Shipped;

        // Act
        var response = await client.PatchAsync($"api/order/{nonExistentOrderId}/status?status={newStatus}", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateOrderStatus_InvalidOrderStatus_ReturnsBadRequest()
    {
        // Arrange
        var client = CreateClient();
        
        var createOrderModel = new OrderCreateModel
        {
            CustomerId = 1,
            OrderEntries = []
        };

        var createOrderResponse = await client.PostAsJsonAsync("api/order", createOrderModel);
        Assert.Equal(HttpStatusCode.Created, createOrderResponse.StatusCode);

        var createdOrder = await createOrderResponse.Content.ReadFromJsonAsync<OrderDetailViewModel>();
        Assert.NotNull(createdOrder);
        var validOrderId = createdOrder.Id;

        const string invalidStatus = "LOKUMSRENS";

        // Act
        var response = await client.PatchAsync($"api/order/{validOrderId}/status?status={invalidStatus}", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateOrderStatusBulk_ValidOrderIdsAndStatus_ReturnsNoContent()
    {
        // Arrange
        var client = CreateClient();
        
        var orderIds = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            var createOrderModel = new OrderCreateModel
            {
                CustomerId = 1,
                OrderEntries = []
            };

            var createOrderResponse = await client.PostAsJsonAsync("api/order", createOrderModel);
            Assert.Equal(HttpStatusCode.Created, createOrderResponse.StatusCode);

            var createdOrder = await createOrderResponse.Content.ReadFromJsonAsync<OrderDetailViewModel>();
            Assert.NotNull(createdOrder);
            orderIds.Add(createdOrder.Id);
        }
        
        const OrderStatus newStatus = OrderStatus.Shipped;
        
        // Act
        var response = await client.PatchAsync($"api/order/status?status={newStatus}", JsonContent.Create(orderIds));

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        foreach (var orderId in orderIds)
        {
            var updatedOrder = await _pgCtxSetup.DbContextInstance.Orders.FindAsync(orderId);
            Assert.NotNull(updatedOrder);
            Assert.Equal(newStatus.ToString().ToLower(), updatedOrder.Status);
        }
    }
    
    [Fact]
    public async Task UpdateOrderStatusBulk_InvalidOrderIds_ReturnsBadRequest()
    {
        // Arrange
        var client = CreateClient();
        var invalidOrderIds = new List<int> { -1, 0 };

        const OrderStatus newStatus = OrderStatus.Shipped;
        
        // Act
        var response = await client.PatchAsync($"api/order/status?status={newStatus}", JsonContent.Create(invalidOrderIds));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateOrderStatusBulk_NoValidOrderIds_ReturnsNotFound()
    {
        // Arrange
        var client = CreateClient();
        var nonExistentOrderIds = new List<int> { int.MaxValue, int.MaxValue - 1 };

        const OrderStatus newStatus = OrderStatus.Shipped;
        
        // Act
        var response = await client.PatchAsync($"api/order/status?status={newStatus}", JsonContent.Create(nonExistentOrderIds));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateOrderStatusBulk_MixedValidAndInvalidOrderIds_ReturnsNoContent()
    {
        // Arrange
        var client = CreateClient();
        
        var createOrderModel = new OrderCreateModel
        {
            CustomerId = 1,
            OrderEntries = []
        };

        var createOrderResponse = await client.PostAsJsonAsync("api/order", createOrderModel);
        Assert.Equal(HttpStatusCode.Created, createOrderResponse.StatusCode);

        var createdOrder = await createOrderResponse.Content.ReadFromJsonAsync<OrderDetailViewModel>();
        Assert.NotNull(createdOrder);
        var validOrderId = createdOrder.Id;
        
        var mixedOrderIds = new List<int> { validOrderId, int.MaxValue };

        const OrderStatus newStatus = OrderStatus.Shipped;
        
        // Act
        var response = await client.PatchAsync($"api/order/status?status={newStatus}", JsonContent.Create(mixedOrderIds));

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        var updatedOrder = await _pgCtxSetup.DbContextInstance.Orders.FindAsync(validOrderId);
        Assert.NotNull(updatedOrder);
        Assert.Equal(newStatus.ToString().ToLower(), updatedOrder.Status);
    }
    
    [Fact]
    public async Task UpdateOrderStatusBulk_DuplicateOrderIds_ReturnsNoContent()
    {
        // Arrange
        var client = CreateClient();
        
        var createOrderModel = new OrderCreateModel
        {
            CustomerId = 1,
            OrderEntries = []
        };

        var createOrderResponse = await client.PostAsJsonAsync("api/order", createOrderModel);
        Assert.Equal(HttpStatusCode.Created, createOrderResponse.StatusCode);

        var createdOrder = await createOrderResponse.Content.ReadFromJsonAsync<OrderDetailViewModel>();
        Assert.NotNull(createdOrder);
        var validOrderId = createdOrder.Id;
        
        var duplicateOrderIds = new List<int> { validOrderId, validOrderId };

        const OrderStatus newStatus = OrderStatus.Shipped;
        
        // Act
        var response = await client.PatchAsync($"api/order/status?status={newStatus}", JsonContent.Create(duplicateOrderIds));

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        var updatedOrder = await _pgCtxSetup.DbContextInstance.Orders.FindAsync(validOrderId);
        Assert.NotNull(updatedOrder);
        Assert.Equal(newStatus.ToString().ToLower(), updatedOrder.Status);
    }
}