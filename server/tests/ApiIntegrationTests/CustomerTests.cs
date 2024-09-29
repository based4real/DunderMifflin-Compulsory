using System.Net;
using System.Net.Http.Json;
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
        var expectedCustomers = _dbFixture.AppDbContext()?.Customers?.OrderBy(c => c.Id).ToList();
        
        Assert.NotNull(expectedCustomers);
        Assert.True(expectedCustomers.Count > 0, "No customers found in database");

        var response = await client.GetAsync("api/customer?orders=false");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var responseData = await response.Content.ReadFromJsonAsync<List<CustomerDetailViewModel>>();
        var sortedResponseData = responseData?.OrderBy(c => c.Id).ToList();
        
        Assert.NotNull(sortedResponseData);
        Assert.Equal(expectedCustomers.Count, sortedResponseData.Count);

        Assert.All(sortedResponseData, expectedCustomer =>
        {
            var actualCustomer = expectedCustomers.Single(c => c.Id == expectedCustomer.Id);
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
        var expectedCustomers = _dbFixture.AppDbContext()
            ?.Customers
            .Include(c => c.Orders)
                .ThenInclude(e => e.OrderEntries)
            .OrderBy(c => c.Id)
            .ToList();
        
        Assert.NotNull(expectedCustomers);
        Assert.True(expectedCustomers.Count > 0, "No customers found in database");

        var response = await client.GetAsync("api/customer?orders=true");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var responseData = await response.Content.ReadFromJsonAsync<List<CustomerOrderDetailViewModel>>();
        var sortedResponseData = responseData?.OrderBy(c => c.Id).ToList();
        
        Assert.NotNull(responseData);
        Assert.Equal(expectedCustomers.Count, responseData.Count);

        Assert.All(sortedResponseData, expectedCustomer =>
        {
            // Tester selve customer data
            var actualCustomer = expectedCustomers.Single(c => c.Id == expectedCustomer.Id);

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
    
    [Fact]
    public async Task GetCustomerOrders_ValidCustomerId_ReturnsPagedOrders()
    {
        // Arrange
        var client = _webFixture.CreateClient();
        var customer = _dbFixture.AppDbContext().Customers.Include(c => c.Orders).FirstOrDefault();
        
        Assert.NotNull(customer);
        
        var customerId = customer.Id;
        var page = 1;
        var pageSize = 2;

        // Act
        var response = await client.GetAsync($"api/customer/{customerId}/orders?page={page}&pageSize={pageSize}");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var responseData = await response.Content.ReadFromJsonAsync<CustomerOrderPagedViewModel>();
        Assert.NotNull(responseData);

        Assert.Equal(customerId, responseData.CustomerDetails.Id);
        Assert.Equal(customer.Orders.Count(), responseData.PagingInfo.TotalItems);
        Assert.Equal(page, responseData.PagingInfo.CurrentPage);
        Assert.Equal(pageSize, responseData.PagingInfo.ItemsPerPage);
    }
}