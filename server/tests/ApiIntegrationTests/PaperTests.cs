using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Service.Models.Requests;
using Service.Models.Responses;
using SharedTestDependencies;

namespace ApiIntegrationTests;

public class PaperTests : IClassFixture<DatabaseFixture>, IClassFixture<WebApplicationFactory<Program>>
{
    private readonly DatabaseFixture _dbFixture;
    private readonly WebApplicationFactory<Program> _webFixture;

    public PaperTests(DatabaseFixture dbFixture, WebApplicationFactory<Program> webFixture)
    {
        _dbFixture = dbFixture;
        _webFixture = webFixture;
    }
    
    [Fact]
    public async Task CreatePaperProperty()
    {
        var client = _webFixture.CreateClient();
        var paperDbList = _dbFixture.AppDbContext().Papers.Take(3).ToList();

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
        
        Assert.All(responseData.PaperPropertyDetails, paper =>
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