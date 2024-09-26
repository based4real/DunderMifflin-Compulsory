using System.Net;
using System.Net.Http.Json;
using DataAccess;
using Microsoft.AspNetCore.Mvc.Testing;
using PgCtx;
using Service;
using Service.Models.Requests;
using Service.Models.Responses;
using SharedTestDependencies;
using Xunit.Abstractions;

namespace ApiIntegrationTests;

public class OrderTest : WebApplicationFactory<Program>
{
    private readonly PgCtxSetup<AppDbContext> _pgCtxSetup = new();
    private readonly ITestOutputHelper _outputHelper;
    
    public OrderTest(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        Environment.SetEnvironmentVariable($"{nameof(AppOptions)}:{nameof(AppOptions.LocalDbConn)}", _pgCtxSetup._postgres.GetConnectionString());
    }

    [Fact]
    public async Task CreateOrderTest()
    {
        var client = CreateClient();
        var customer = TestObjects.Customer();
        _pgCtxSetup.DbContextInstance.Customers.Add(customer);
        _pgCtxSetup.DbContextInstance.SaveChanges();
        
        var paper = TestObjects.Paper();
        _pgCtxSetup.DbContextInstance.Papers.Add(paper);
        _pgCtxSetup.DbContextInstance.SaveChanges();
        
        var orderEntry = TestObjects.OrderEntry(paper);
        var order = TestObjects.Order(customer, [orderEntry]);
        _pgCtxSetup.DbContextInstance.Orders.Add(order);
        _pgCtxSetup.DbContextInstance.SaveChanges();
        
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
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
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