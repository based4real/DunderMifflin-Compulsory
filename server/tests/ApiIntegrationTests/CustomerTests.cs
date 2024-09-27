using System.Net;
using System.Net.Http.Json;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Service.Models.Responses;
using SharedTestDependencies;

namespace ApiIntegrationTests;

public class CustomerTests : IClassFixture<DatabaseFixture>, IClassFixture<WebApplicationFactory<Program>>
{
    private readonly DatabaseFixture _dbFixture;
    private readonly WebApplicationFactory<Program> _webFixture;
    
    public CustomerTests(DatabaseFixture dbFixture, WebApplicationFactory<Program> webFixture)
    {
        _dbFixture = dbFixture;
        _webFixture = webFixture;
    }

    [Fact]
    public async Task GetSingleCustomer()
    {
        var client = _webFixture.CreateClient();
        var expectedCustomer = _dbFixture.AppDbContext().Customers.First();
        Assert.NotNull(expectedCustomer);

        var response = await client.GetAsync($"api/customer/{expectedCustomer.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseData = await response.Content.ReadFromJsonAsync<CustomerDetailViewModel>();

        Assert.NotNull(responseData);
        
        Assert.Equal(expectedCustomer.Id, responseData.Id);
        Assert.Equal(expectedCustomer.Name, responseData.Name);
        Assert.Equal(expectedCustomer.Email, responseData.Email);
        Assert.Equal(expectedCustomer.Phone, responseData.Phone);
        Assert.Equal(expectedCustomer.Address, responseData.Address);
    }

    [Fact]
    public async Task All()
    {
        var client = _webFixture.CreateClient();
        var exceptedCustomers = _dbFixture.AppDbContext()?.Customers?.ToList();
        
        Assert.NotNull(exceptedCustomers);
        Assert.True(exceptedCustomers.Count > 0, "No customers found in database");

        var response = await client.GetAsync("api/customer?orders=false");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var responseData = await response.Content.ReadFromJsonAsync<List<CustomerDetailViewModel>>();

        Assert.NotNull(responseData);
        Assert.Equal(exceptedCustomers.Count, responseData.Count);

        Assert.All(responseData, expectedCustomer =>
        {
            var actualCustomer = exceptedCustomers.Single(c => c.Id == expectedCustomer.Id);
            Assert.NotNull(actualCustomer);
        
            Assert.Equal(expectedCustomer.Id, actualCustomer.Id);
            Assert.Equal(expectedCustomer.Name, actualCustomer.Name);
            Assert.Equal(expectedCustomer.Email, actualCustomer.Email);
            Assert.Equal(expectedCustomer.Phone, actualCustomer.Phone);
        });
    }

    [Fact]
    public async Task AllWithHistory()
    {
        var client = _webFixture.CreateClient();
        var exceptedCustomers = _dbFixture.AppDbContext()
            ?.Customers
            .Include(c => c.Orders)
                .ThenInclude(e => e.OrderEntries)
            .ToList();
        
        Assert.NotNull(exceptedCustomers);
        Assert.True(exceptedCustomers.Count > 0, "No customers found in database");

        var response = await client.GetAsync("api/customer?orders=true");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var responseData = await response.Content.ReadFromJsonAsync<List<CustomerOrderDetailViewModel>>();

        Assert.NotNull(responseData);
        Assert.Equal(exceptedCustomers.Count, responseData.Count);

        Assert.All(responseData, expectedCustomer =>
        {
            // Tester selve customer data
            var actualCustomer = exceptedCustomers.Single(c => c.Id == expectedCustomer.Id);

            Assert.Equal(expectedCustomer.Id, actualCustomer.Id);
            Assert.Equal(expectedCustomer.Name, actualCustomer.Name);
            Assert.Equal(expectedCustomer.Email, actualCustomer.Email);
            
            // Sorter expected og actual udfra ID for at sikre der ikke er mismatch på ids
            var expectedOrders = expectedCustomer.Orders.OrderBy(o => o.Id).ToList();
            var actualOrders = actualCustomer.Orders.OrderBy(o => o.Id).ToList();
            
            Assert.Equal(expectedOrders.Count, actualOrders.Count);
            Assert.All(expectedOrders, expectedOrder =>
            {
                // Tjekker selve ordre på customeren
                var actualOrder = actualOrders.Single(o => o.Id == expectedOrder.Id);
                Assert.NotNull(actualOrder);
                Assert.Equal(expectedOrder.Id, actualOrder.Id);
                Assert.Equal(expectedOrder.OrderDate, actualOrder.OrderDate);
                Assert.Equal(expectedOrder.Status, actualOrder.Status);
                
                // Sorter expected og actual udfra ID for at sikre der ikke er mismatch på ids
                var expectedEntries = expectedOrder.Entry.OrderBy(e => e.Id).ToList();
                var actualEntries = actualOrder.OrderEntries.OrderBy(e => e.Id).ToList();

                Assert.Equal(expectedEntries.Count, actualEntries.Count);
                Assert.All(expectedEntries, expectedEntry =>
                {
                    // Tjekker selve entry for ordren
                    var actualEntry = actualEntries.Single(e => e.Id == expectedEntry.Id);
                    Assert.NotNull(actualEntry);

                    Assert.Equal(expectedEntry.Id, actualEntry.Id);
                    Assert.Equal(expectedEntry.Quantity, actualEntry.Quantity);
                });
            });
        });
    }
}