using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Service.Models.Requests;
using Service.Models.Responses;

namespace ApiIntegrationTests;

public class OrderTest : WebApplicationFactory<Program>
{
    [Fact]
    public async Task CreateOrderTest()
    {
        var client = CreateClient();

        var createOrderModel = new OrderCreateModel
        {
            CustomerId = 1,
            OrderEntries = new List<OrderCreateEntryModel>
            {
                new OrderCreateEntryModel
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