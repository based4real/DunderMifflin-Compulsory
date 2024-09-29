using System.Net;
using System.Net.Http.Json;
using DataAccess;
using Microsoft.AspNetCore.Mvc.Testing;
using PgCtx;
using Service;
using Service.Models.Requests;
using Service.Models.Responses;
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
}