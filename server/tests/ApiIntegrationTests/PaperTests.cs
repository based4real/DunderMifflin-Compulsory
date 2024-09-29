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

public class PaperTests : WebApplicationFactory<Program>
{
    private readonly PgCtxSetup<AppDbContext> _pgCtxSetup = new();

    public PaperTests()
    {
        Environment.SetEnvironmentVariable($"{nameof(AppOptions)}:{nameof(AppOptions.LocalDbConn)}", _pgCtxSetup._postgres.GetConnectionString());
        
        SeedDatabase();
    }
    
    [Fact]
    public async Task CreatePaperProperty_WithoutPapers()
    {
        var client = CreateClient();
        
        var createPropertyModel = new PaperPropertyCreateModel
        {
            name = "A4"
        };
        
        var response = await client.PostAsJsonAsync("api/paper/property", createPropertyModel);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var responseData = await response.Content.ReadFromJsonAsync<PaperPropertyDetailViewModel>();
        
        Assert.NotNull(responseData);
        Assert.NotEqual(0, responseData.Id);
        Assert.True(responseData.Name == createPropertyModel.name);

        Assert.True(responseData.PaperPropertyDetails?.Count == 0);
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
    public async Task CreatePaperProperty_WithPapers()
    {
        var client = CreateClient();
        var paperDbList = _pgCtxSetup.DbContextInstance.Papers.OrderBy(p => p.Id).Take(3).ToList();

        Assert.NotNull(paperDbList);
        Assert.Equal(3, paperDbList.Count);
        
        var createPropertyModel = new PaperPropertyCreateModel
        {
            name = "A5",
            PapersId = paperDbList.Select(p => p.Id).ToList()
        };
        
        var response = await client.PostAsJsonAsync("api/paper/property", createPropertyModel);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var responseData = await response.Content.ReadFromJsonAsync<PaperPropertyDetailViewModel>();
        
        Assert.NotNull(responseData);
        Assert.NotEqual(0, responseData.Id);
        Assert.True(responseData.Name == createPropertyModel.name);

        Assert.NotNull(responseData.PaperPropertyDetails);
        Assert.NotEmpty(responseData.PaperPropertyDetails);
        
        var sortedResponsePapers = responseData.PaperPropertyDetails.OrderBy(p => p.Id).ToList();
        
        Assert.All(sortedResponsePapers, paper =>
        {
            Assert.NotNull(paper);

            var correspondingPaper = paperDbList.SingleOrDefault(p => p.Id == paper.Id);
            Assert.NotNull(correspondingPaper);

            Assert.Equal(correspondingPaper.Id, paper.Id);
            Assert.Equal(correspondingPaper.Name, paper.Name);
            Assert.Equal(correspondingPaper.Stock, paper.Stock);
        });
    }
}