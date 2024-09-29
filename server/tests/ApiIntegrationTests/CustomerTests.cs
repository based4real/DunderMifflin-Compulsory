using System.Net;
using System.Net.Http.Json;
using DataAccess;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using PgCtx;
using Service;
using Service.Models.Responses;
using SharedTestDependencies;

namespace ApiIntegrationTests;

public class CustomerTests : WebApplicationFactory<Program>
{
    private readonly PgCtxSetup<AppDbContext> _pgCtxSetup = new();
    
    public CustomerTests()
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
    public async Task GetSingleCustomer()
    {
        var client = CreateClient();
        var expectedCustomer = _pgCtxSetup.DbContextInstance.Customers.First();
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
        var client = CreateClient();
        var expectedCustomers = _pgCtxSetup.DbContextInstance?.Customers?.OrderBy(c => c.Id).ToList();
        
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
        var client = CreateClient();
        var expectedCustomers = _pgCtxSetup.DbContextInstance
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
    public async Task GetCustomerWithOrders_ValidCustomerId_ReturnsPagedOrders()
    {
        // Arrange
        var client = CreateClient();
        var customer = _pgCtxSetup.DbContextInstance.Customers.Include(c => c.Orders).FirstOrDefault();
        
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
    
    [Fact]
    public async Task GetCustomerWithOrders_InvalidCustomerId_ReturnsNotFound()
    {
        // Arrange
        var client = CreateClient();
        var invalidCustomerId = int.MaxValue; // Antager at dette ID ikke eksisterer i DB
        
        // Act
        var response = await client.GetAsync($"api/customer/{invalidCustomerId}/orders?page=1&pageSize=10");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task GetCustomerWithOrders_ValidCustomerId_NoOrders_ReturnsEmptyOrders()
    {
        // Arrange
        var client = CreateClient();
        
        var customerWithoutOrders = TestObjects.Customer();
        _pgCtxSetup.DbContextInstance.Customers.Add(customerWithoutOrders);
        _pgCtxSetup.DbContextInstance.SaveChanges();
        
        // Act
        var response = await client.GetAsync($"api/customer/{customerWithoutOrders.Id}/orders?page=1&pageSize=10");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var responseData = await response.Content.ReadFromJsonAsync<CustomerOrderPagedViewModel>();
        Assert.NotNull(responseData);
        Assert.Empty(responseData.CustomerDetails.Orders);
        Assert.Equal(customerWithoutOrders.Id, responseData.CustomerDetails.Id);
    }
    
    [Fact]
    public async Task GetCustomerWithOrders_ValidCustomerId_DifferentPageSizes()
    {
        // Arrange
        var client = CreateClient();
        var customer = _pgCtxSetup.DbContextInstance.Customers.Include(c => c.Orders).FirstOrDefault();
        Assert.NotNull(customer);
        
        var customerId = customer.Id;
        var page = 1;
        var pageSizes = new[] { 1, 2, 5 }; 
        
        foreach (var pageSize in pageSizes)
        {
            // Act
            var response = await client.GetAsync($"api/customer/{customerId}/orders?page={page}&pageSize={pageSize}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseData = await response.Content.ReadFromJsonAsync<CustomerOrderPagedViewModel>();
            Assert.NotNull(responseData);
            Assert.Equal(pageSize, responseData.PagingInfo.ItemsPerPage);
            Assert.True(responseData.CustomerDetails.Orders.Count() <= pageSize);
        }
    }
    
    [Fact]
    public async Task GetCustomerOrder_ValidCustomerAndOrderId_ReturnsOrderDetails()
    {
        // Arrange
        var client = CreateClient();
        var customer = _pgCtxSetup.DbContextInstance.Customers.Include(c => c.Orders).ThenInclude(o => o.OrderEntries).FirstOrDefault();
        
        Assert.NotNull(customer);
        var order = customer.Orders.FirstOrDefault();
        Assert.NotNull(order);

        // Act
        var response = await client.GetAsync($"api/customer/{customer.Id}/orders/{order.Id}");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseData = await response.Content.ReadFromJsonAsync<OrderDetailViewModel>();
        Assert.NotNull(responseData);
        
        Assert.Equal(order.Id, responseData.Id);
        Assert.Equal(order.OrderDate, responseData.OrderDate);
        Assert.Equal(order.Status, responseData.Status);
        Assert.Equal(order.DeliveryDate, responseData.DeliveryDate);
        Assert.Equal(order.TotalAmount, responseData.TotalPrice);
        Assert.Equal(order.OrderEntries.Count, responseData.Entry.Count());
    }
}