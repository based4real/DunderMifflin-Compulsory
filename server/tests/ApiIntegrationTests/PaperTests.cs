using System.Net;
using System.Net.Http.Json;
using DataAccess;
using DataAccess.Models;
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
            Name = "A4"
        };
        
        var response = await client.PostAsJsonAsync("api/paper/property", createPropertyModel);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var responseData = await response.Content.ReadFromJsonAsync<PaperPropertyDetailViewModel>();
        
        Assert.NotNull(responseData);
        Assert.NotEqual(0, responseData.Id);
        Assert.True(responseData.Name == createPropertyModel.Name);

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
            Name = "A5",
            PapersId = paperDbList.Select(p => p.Id).ToList()
        };
        
        var response = await client.PostAsJsonAsync("api/paper/property", createPropertyModel);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var responseData = await response.Content.ReadFromJsonAsync<PaperPropertyDetailViewModel>();
        
        Assert.NotNull(responseData);
        Assert.NotEqual(0, responseData.Id);
        Assert.True(responseData.Name == createPropertyModel.Name);

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
    
    [Fact]
    public async Task GetAllPapers_WithDefaultParameters_ReturnsPapers()
    {
        // Arrange
        var client = CreateClient();
    
        // Act
        var response = await client.GetAsync("api/paper");
    
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    
        var responseData = await response.Content.ReadFromJsonAsync<PaperPagedViewModel>();
        Assert.NotNull(responseData);
        Assert.NotNull(responseData.Papers);
        Assert.NotEmpty(responseData.Papers);
        Assert.Equal(1, responseData.PagingInfo.CurrentPage);
        Assert.Equal(10, responseData.PagingInfo.ItemsPerPage);
    }
    
    [Fact]
    public async Task GetAllPapers_WithSpecificPageAndPageSize_ReturnsPapers()
    {
        // Arrange
        var client = CreateClient();

        const int page = 2;
        const int pageSize = 1;

        // Act
        var response = await client.GetAsync($"api/paper?page={page}&pageSize={pageSize}");
    
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    
        var responseData = await response.Content.ReadFromJsonAsync<PaperPagedViewModel>();
        Assert.NotNull(responseData);
        Assert.NotNull(responseData.Papers);
        Assert.NotEmpty(responseData.Papers);
        Assert.Equal(page, responseData.PagingInfo.CurrentPage);
        Assert.Equal(pageSize, responseData.PagingInfo.ItemsPerPage);
    }
    
    [Fact]
    public async Task GetAllPapers_WithMinimumPageSize_ReturnsPapers()
    {
        // Arrange
        var client = CreateClient();
        
        const int pageSize = 1;

        // Act
        var response = await client.GetAsync($"api/paper?page=1&pageSize={pageSize}");
    
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    
        var responseData = await response.Content.ReadFromJsonAsync<PaperPagedViewModel>();
        Assert.NotNull(responseData);
        Assert.NotNull(responseData.Papers);
        Assert.Single(responseData.Papers);
        Assert.Equal(pageSize, responseData.PagingInfo.ItemsPerPage);
    }
    
    [Fact]
    public async Task GetAllPapers_WithMaximumPageSize_ReturnsPapers()
    {
        // Arrange
        var client = CreateClient();
        
        const int pageSize = 1000;

        // Act
        var response = await client.GetAsync($"api/paper?page=1&pageSize={pageSize}");
    
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    
        var responseData = await response.Content.ReadFromJsonAsync<PaperPagedViewModel>();
        Assert.NotNull(responseData);
        Assert.NotNull(responseData.Papers);
        Assert.True(responseData.Papers.Count <= pageSize);
        Assert.Equal(pageSize, responseData.PagingInfo.ItemsPerPage);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1001)]
    public async Task GetAllPapers_WithInvalidPageSize_ReturnsBadRequest(int pageSize)
    {
        // Arrange
        var client = CreateClient();

        // Act
        var response = await client.GetAsync($"api/paper?page=1&pageSize={pageSize}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public async Task GetAllPapers_WithInvalidPageNumber_ReturnsBadRequest(int page)
    {
        // Arrange
        var client = CreateClient();

        // Act
        var response = await client.GetAsync($"api/paper?page={page}&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task GetAllPapers_WithPageHigherThanAvailable_ReturnsEmptyList()
    {
        // Arrange
        var client = CreateClient();

        const int page = 10; // Page number greater than available pages

        // Act
        var response = await client.GetAsync($"api/paper?page={page}&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    
        var responseData = await response.Content.ReadFromJsonAsync<PaperPagedViewModel>();
        Assert.NotNull(responseData);
        Assert.NotNull(responseData.Papers);
        Assert.Empty(responseData.Papers);
    }
    
    [Fact]
    public async Task GetAllPapers_WithSearchQuery_ReturnsMatchingPapers()
    {
        // Arrange
        var testPaper = new PaperCreateModel
        {
            Name = "stor fed kartoffel",
            Price = 19.99,
            PropertyIds = [],
            Stock = 50
        }.ToPaper();
        
        await _pgCtxSetup.DbContextInstance.Papers.AddAsync(testPaper);
        await _pgCtxSetup.DbContextInstance.SaveChangesAsync();
        
        var client = CreateClient();
    
        const string searchQuery = "stor fed kartoffel";

        // Act
        var response = await client.GetAsync($"api/paper?page=1&pageSize=10&search={searchQuery}");
    
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    
        var responseData = await response.Content.ReadFromJsonAsync<PaperPagedViewModel>();
        Assert.NotNull(responseData);
        Assert.NotEmpty(responseData.Papers);

        Assert.All(responseData.Papers, paper => 
        {
            Assert.Contains(searchQuery.ToLower(), paper.Name?.ToLower());
        });
    }
    
    [Fact]
    public async Task GetAllPapers_WithSearchQuery_CaseInsensitive_ReturnsMatchingPapers()
    {
        // Arrange
        var testPaper = new PaperCreateModel
        {
            Name = "sToR fEd KARTOFFEL",
            Price = 19.99,
            PropertyIds = [],
            Stock = 50
        }.ToPaper();
        
        await _pgCtxSetup.DbContextInstance.Papers.AddAsync(testPaper);
        await _pgCtxSetup.DbContextInstance.SaveChangesAsync();
        
        var client = CreateClient();

        const string searchQuery = "stor fed kartoffel";

        // Act
        var response = await client.GetAsync($"api/paper?page=1&pageSize=10&search={searchQuery}");
    
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    
        var responseData = await response.Content.ReadFromJsonAsync<PaperPagedViewModel>();
        Assert.NotNull(responseData);
        Assert.NotEmpty(responseData.Papers);

        Assert.All(responseData.Papers, paper => 
        {
            Assert.Contains(searchQuery.ToLower(), paper.Name?.ToLower());
        });
    }
    
    [Fact]
    public async Task GetAllPapers_WithDiscontinuedTrue_ReturnsOnlyDiscontinuedPapers()
    {
        // Arrange
        var testPaper = new Paper
        {
            Name = "Kartoffel",
            Price = 19.99,
            Discontinued = true,
            Stock = 50
        };
        
        await _pgCtxSetup.DbContextInstance.Papers.AddAsync(testPaper);
        await _pgCtxSetup.DbContextInstance.SaveChangesAsync();
        
        var client = CreateClient();

        // Act
        var response = await client.GetAsync("api/paper?page=1&pageSize=10&discontinued=true");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    
        var responseData = await response.Content.ReadFromJsonAsync<PaperPagedViewModel>();
        Assert.NotNull(responseData);
        Assert.NotEmpty(responseData.Papers);

        Assert.All(responseData.Papers, paper => 
        {
            Assert.True(paper.Discontinued);
        });
    }
    
    [Fact]
    public async Task GetAllPapers_WithDiscontinuedFalse_ReturnsOnlyNonDiscontinuedPapers()
    {
        // Arrange
        var testPaper = new Paper
        {
            Name = "Kartoffel",
            Price = 19.99,
            Discontinued = false,
            Stock = 50
        };
        
        await _pgCtxSetup.DbContextInstance.Papers.AddAsync(testPaper);
        await _pgCtxSetup.DbContextInstance.SaveChangesAsync();
        
        var client = CreateClient();

        // Act
        var response = await client.GetAsync("api/paper?page=1&pageSize=10&discontinued=false");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    
        var responseData = await response.Content.ReadFromJsonAsync<PaperPagedViewModel>();
        Assert.NotNull(responseData);
        Assert.NotEmpty(responseData.Papers);

        Assert.All(responseData.Papers, paper => 
        {
            Assert.False(paper.Discontinued);
        });
    }
    
    [Theory]
    [InlineData("Price", "Desc")]
    [InlineData("Price", "Asc")]
    [InlineData("Name", "Desc")]
    [InlineData("Name", "Asc")]
    [InlineData("Stock", "Desc")]
    [InlineData("Stock", "Asc")]
    public async Task GetAllPapers_WithOrderByAndSortBy_ReturnsSortedPapers(string orderBy, string sortBy)
    {
        // Arrange
        var client = CreateClient();
    
        // Act
        var response = await client.GetAsync($"api/paper?orderBy={orderBy}&sortBy={sortBy}");
    
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseData = await response.Content.ReadFromJsonAsync<PaperPagedViewModel>();
        Assert.NotNull(responseData);
        Assert.NotNull(responseData.Papers);
        Assert.NotEmpty(responseData.Papers);

        var papers = responseData.Papers;
    
        if (orderBy == "Price")
        {
            var sortedPapers = sortBy == "Desc" ? papers.OrderByDescending(p => p.Price).ToList() : papers.OrderBy(p => p.Price).ToList();
            Assert.True(papers.SequenceEqual(sortedPapers, new PaperPriceComparer()));
        }
        else if (orderBy == "Name")
        {
            var sortedPapers = sortBy == "Desc" ? papers.OrderByDescending(p => p.Name).ToList() : papers.OrderBy(p => p.Name).ToList();
            Assert.True(papers.SequenceEqual(sortedPapers, new PaperNameComparer()));
        }
        else if (orderBy == "Stock")
        {
            var sortedPapers = sortBy == "Desc" ? papers.OrderByDescending(p => p.Stock).ToList() : papers.OrderBy(p => p.Stock).ToList();
            Assert.True(papers.SequenceEqual(sortedPapers, new PaperStockComparer()));
        }
    }
    
    [Fact]
    public async Task GetAllPapers_WithDefaultOrdering_ReturnsPapersSortedById()
    {
        // Arrange
        var client = CreateClient();

        // Act
        var response = await client.GetAsync("api/paper");
    
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseData = await response.Content.ReadFromJsonAsync<PaperPagedViewModel>();
        Assert.NotNull(responseData);
        Assert.NotNull(responseData.Papers);
        Assert.NotEmpty(responseData.Papers);

        var sortedPapers = responseData.Papers.OrderBy(p => p.Id).ToList();
        Assert.True(responseData.Papers.SequenceEqual(sortedPapers, new PaperIdComparer()));
    }
    
    [Fact]
    public async Task GetAllPapers_WithSinglePropertyFilter_ReturnsFilteredPapers()
    {
        // Arrange
        var client = CreateClient();

        // Lav ny property
        var createPropertyModel = new PaperPropertyCreateModel
        {
            Name = "Recycled Kartoffel"
        };
    
        var propertyResponse = await client.PostAsJsonAsync("api/paper/property", createPropertyModel);
        Assert.Equal(HttpStatusCode.Created, propertyResponse.StatusCode);
    
        var propertyData = await propertyResponse.Content.ReadFromJsonAsync<PaperPropertyDetailViewModel>();
        Assert.NotNull(propertyData);
        var propertyId = propertyData.Id;

        // Lav nyt paper med ovenstående property
        var createPaperModel = new PaperCreateModel
        {
            Name = "A4 Kartoffel papir",
            Price = 10.99,
            Stock = 100,
            PropertyIds = new List<int> { propertyId }
        };

        var paperResponse = await client.PostAsJsonAsync("api/paper", new List<PaperCreateModel> { createPaperModel });
        Assert.Equal(HttpStatusCode.Created, paperResponse.StatusCode);
    
        var paperData = await paperResponse.Content.ReadFromJsonAsync<List<PaperDetailViewModel>>();
        Assert.NotNull(paperData);
        Assert.Contains(paperData.First().Properties, property => property.Id == propertyId);

        // Filter papir på property
        var response = await client.GetAsync($"api/paper?filter={propertyId}");
    
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseData = await response.Content.ReadFromJsonAsync<PaperPagedViewModel>();
        Assert.NotNull(responseData);
        Assert.NotEmpty(responseData.Papers);

        Assert.All(responseData.Papers, paper => 
        {
            Assert.Contains(paper.Properties, property => property.Id == propertyId);
        });
    }

    [Fact]
    public async Task GetAllPapers_WithMultiplePropertyFilterOr_ReturnsPapersHavingAnyProperties()
    {
        // Arrange
        var client = CreateClient();

        // Opret properties
        var createPropertyModel1 = new PaperPropertyCreateModel { Name = "Recycled COLA" };
        var createPropertyModel2 = new PaperPropertyCreateModel { Name = "Glossy KAKAO" };

        var propertyResponse1 = await client.PostAsJsonAsync("api/paper/property", createPropertyModel1);
        var propertyResponse2 = await client.PostAsJsonAsync("api/paper/property", createPropertyModel2);

        Assert.Equal(HttpStatusCode.Created, propertyResponse1.StatusCode);
        Assert.Equal(HttpStatusCode.Created, propertyResponse2.StatusCode);

        var propertyData1 = await propertyResponse1.Content.ReadFromJsonAsync<PaperPropertyDetailViewModel>();
        var propertyData2 = await propertyResponse2.Content.ReadFromJsonAsync<PaperPropertyDetailViewModel>();

        Assert.NotNull(propertyData1);
        Assert.NotNull(propertyData2);

        var propertyId1 = propertyData1.Id;
        var propertyId2 = propertyData2.Id;

        // Lav papir
        var createPaperModel1 = new PaperCreateModel
        {
            Name = "Recycled COLA Paper",
            Price = 12.99,
            Stock = 50,
            PropertyIds = new List<int> { propertyId1 }
        };

        var createPaperModel2 = new PaperCreateModel
        {
            Name = "Glossy KAKAO Paper",
            Price = 14.99,
            Stock = 70,
            PropertyIds = new List<int> { propertyId2 }
        };

        var paperResponse = await client.PostAsJsonAsync("api/paper", new List<PaperCreateModel> { createPaperModel1, createPaperModel2 });
        Assert.Equal(HttpStatusCode.Created, paperResponse.StatusCode);

        // Filer med 'OR'
        var response = await client.GetAsync($"api/paper?filter={propertyId1},{propertyId2}&filterType=Or");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseData = await response.Content.ReadFromJsonAsync<PaperPagedViewModel>();
        Assert.NotNull(responseData);
        Assert.NotEmpty(responseData.Papers);

        Assert.Contains(responseData.Papers, paper => paper.Name == "Recycled COLA Paper");
        Assert.Contains(responseData.Papers, paper => paper.Name == "Glossy KAKAO Paper");
    }
    
    [Fact]
    public async Task GetAllPapers_WithMultiplePropertyFilterAnd_ReturnsPapersHavingAllProperties()
    {
        // Arrange
        var client = CreateClient();

        // Lav properties
        var createPropertyModel1 = new PaperPropertyCreateModel { Name = "Recycled COLA" };
        var createPropertyModel2 = new PaperPropertyCreateModel { Name = "Glossy KAKAO" };

        var propertyResponse1 = await client.PostAsJsonAsync("api/paper/property", createPropertyModel1);
        var propertyResponse2 = await client.PostAsJsonAsync("api/paper/property", createPropertyModel2);

        Assert.Equal(HttpStatusCode.Created, propertyResponse1.StatusCode);
        Assert.Equal(HttpStatusCode.Created, propertyResponse2.StatusCode);

        var propertyData1 = await propertyResponse1.Content.ReadFromJsonAsync<PaperPropertyDetailViewModel>();
        var propertyData2 = await propertyResponse2.Content.ReadFromJsonAsync<PaperPropertyDetailViewModel>();

        Assert.NotNull(propertyData1);
        Assert.NotNull(propertyData2);

        var propertyId1 = propertyData1.Id;
        var propertyId2 = propertyData2.Id;

        // Lav papir
        var createPaperModel = new PaperCreateModel
        {
            Name = "Recycled Glossy KAKAO COLA Paper",
            Price = 15.99,
            Stock = 30,
            PropertyIds = new List<int> { propertyId1, propertyId2 }
        };

        var paperResponse = await client.PostAsJsonAsync("api/paper", new List<PaperCreateModel> { createPaperModel });
        Assert.Equal(HttpStatusCode.Created, paperResponse.StatusCode);

        // Filter med 'AND'
        var response = await client.GetAsync($"api/paper?filter={propertyId1},{propertyId2}&filterType=And");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseData = await response.Content.ReadFromJsonAsync<PaperPagedViewModel>();
        Assert.NotNull(responseData);
        Assert.NotEmpty(responseData.Papers);

        Assert.All(responseData.Papers, paper => 
        {
            Assert.Contains(paper.Properties, property => property.Id == propertyId1);
            Assert.Contains(paper.Properties, property => property.Id == propertyId2);
        });
    }
    
    [Fact]
    public async Task GetAllPapers_CombinedFilteringAndSorting_ReturnsExpectedResult()
    {
        // Arrange
        var papersToAdd = new List<Paper>
        {
            new()
            {
                Name = "A4 Banana Scented Paper",
                Price = 12.99,
                Stock = 50,
                Discontinued = false
            },
            new()
            {
                Name = "A4 Unicorn Sparkle Paper",
                Price = 15.99,
                Stock = 30,
                Discontinued = true
            },
            new()
            {
                Name = "A4 Banana Scented Unicorn Sparkle Paper",
                Price = 18.99,
                Stock = 20,
                Discontinued = false
            }
        };

        await _pgCtxSetup.DbContextInstance.Papers.AddRangeAsync(papersToAdd);
        await _pgCtxSetup.DbContextInstance.SaveChangesAsync();
        
        var addedPapers = _pgCtxSetup.DbContextInstance.Papers.OrderBy(p => p.Id).ToList();
        var paperId1 = addedPapers.First(p => p.Name.Contains("Banana Scented")).Id;
        var paperId2 = addedPapers.First(p => p.Name.Contains("Unicorn Sparkle")).Id;
        
        var client = CreateClient();
        
        // Lav properties
        var createPropertyModel1 = new PaperPropertyCreateModel { Name = "Banana Scented", PapersId = new List<int> { paperId1 } };
        var createPropertyModel2 = new PaperPropertyCreateModel { Name = "Unicorn Sparkles", PapersId = new List<int> { paperId2 } };

        var propertyResponse1 = await client.PostAsJsonAsync("api/paper/property", createPropertyModel1);
        var propertyResponse2 = await client.PostAsJsonAsync("api/paper/property", createPropertyModel2);

        Assert.Equal(HttpStatusCode.Created, propertyResponse1.StatusCode);
        Assert.Equal(HttpStatusCode.Created, propertyResponse2.StatusCode);

        var propertyData1 = await propertyResponse1.Content.ReadFromJsonAsync<PaperPropertyDetailViewModel>();
        var propertyData2 = await propertyResponse2.Content.ReadFromJsonAsync<PaperPropertyDetailViewModel>();

        Assert.NotNull(propertyData1);
        Assert.NotNull(propertyData2);

        var propertyId1 = propertyData1.Id;
        var propertyId2 = propertyData2.Id;
        
        // Kombiner search, discontinued, orderBy, sortBy, og filter
        var response = await client.GetAsync($"api/paper?search=Banana&discontinued=false&orderBy=Price&sortBy=Asc&filter={propertyId1}&filterType=Or");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseData = await response.Content.ReadFromJsonAsync<PaperPagedViewModel>();
        Assert.NotNull(responseData);
        Assert.NotEmpty(responseData.Papers);
        
        Assert.All(responseData.Papers, paper =>
        {
            Assert.Contains("Banana", paper.Name);
            Assert.False(paper.Discontinued);
            Assert.Contains(paper.Properties, property => property.Id == propertyId1);
        });
        
        var sortedPapers = responseData.Papers.OrderBy(p => p.Price).ToList();
        Assert.True(responseData.Papers.SequenceEqual(sortedPapers, new PaperPriceComparer()));
    }
    
    [Fact]
    public async Task Discontinue_ValidPaperId_ReturnsNoContent()
    {
        // Arrange
        var client = CreateClient();
        var createPaperModel = new PaperCreateModel
        {
            Name = "Test Paper",
            Price = 10.99,
            Stock = 100,
            PropertyIds = []
        };

        var createPaperResponse = await client.PostAsJsonAsync("api/paper", new List<PaperCreateModel> { createPaperModel });
        Assert.Equal(HttpStatusCode.Created, createPaperResponse.StatusCode);

        var paperData = await createPaperResponse.Content.ReadFromJsonAsync<List<PaperDetailViewModel>>();
        Assert.NotNull(paperData);
        var validPaperId = paperData.First().Id;

        // Act
        var response = await client.PatchAsync($"api/paper/{validPaperId}/discontinue", null);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var updatedPaper = await _pgCtxSetup.DbContextInstance.Papers.FindAsync(validPaperId);
        Assert.NotNull(updatedPaper);
        Assert.True(updatedPaper.Discontinued);
    }
    
    [Fact]
    public async Task Discontinue_InvalidPaperId_ReturnsBadRequest()
    {
        // Arrange
        var client = CreateClient();
        const int invalidPaperId = -1;

        // Act
        var response = await client.PatchAsync($"api/paper/{invalidPaperId}/discontinue", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task Discontinue_NonExistentPaperId_ReturnsNotFound()
    {
        // Arrange
        var client = CreateClient();
        const int nonExistentPaperId = int.MaxValue;

        // Act
        var response = await client.PatchAsync($"api/paper/{nonExistentPaperId}/discontinue", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task DiscontinueBulk_ValidPaperIds_ReturnsNoContent()
    {
        // Arrange
        var client = CreateClient();

        var papersToCreate = new List<PaperCreateModel>
        {
            new() { Name = "Paper 1", Price = 5.99, Stock = 100, PropertyIds = [] },
            new() { Name = "Paper 2", Price = 7.99, Stock = 200, PropertyIds = [] }
        };

        var createPapersResponse = await client.PostAsJsonAsync("api/paper", papersToCreate);
        Assert.Equal(HttpStatusCode.Created, createPapersResponse.StatusCode);

        var createdPapers = await createPapersResponse.Content.ReadFromJsonAsync<List<PaperDetailViewModel>>();
        Assert.NotNull(createdPapers);
        var paperIds = createdPapers.Select(p => p.Id).ToList();

        // Act
        var response = await client.PatchAsync("api/paper/discontinue", JsonContent.Create(paperIds));

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        foreach (var paperId in paperIds)
        {
            var updatedPaper = await _pgCtxSetup.DbContextInstance.Papers.FindAsync(paperId);
            Assert.NotNull(updatedPaper);
            Assert.True(updatedPaper.Discontinued);
        }
    }
    
    [Fact]
    public async Task DiscontinueBulk_InvalidPaperIds_ReturnsBadRequest()
    {
        // Arrange
        var client = CreateClient();
        var invalidPaperIds = new List<int> { -1, 0, -5 };

        // Act
        var response = await client.PatchAsync("api/paper/discontinue", JsonContent.Create(invalidPaperIds));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task DiscontinueBulk_NoValidPaperIds_ReturnsNotFound()
    {
        // Arrange
        var client = CreateClient();
        var nonExistentPaperIds = new List<int> { int.MaxValue, int.MaxValue - 1 };

        // Act
        var response = await client.PatchAsync("api/paper/discontinue", JsonContent.Create(nonExistentPaperIds));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task DiscontinueBulk_MixedValidAndInvalidPaperIds_ReturnsNoContent()
    {
        // Arrange
        var client = CreateClient();

        var createPaperModel = new PaperCreateModel
        {
            Name = "Valid Paper",
            Price = 6.99,
            Stock = 150,
            PropertyIds = []
        };

        var createPaperResponse = await client.PostAsJsonAsync("api/paper", new List<PaperCreateModel> { createPaperModel });
        Assert.Equal(HttpStatusCode.Created, createPaperResponse.StatusCode);

        var createdPaper = await createPaperResponse.Content.ReadFromJsonAsync<List<PaperDetailViewModel>>();
        Assert.NotNull(createdPaper);
        var validPaperId = createdPaper.First().Id;

        var mixedPaperIds = new List<int> { validPaperId, -1, int.MaxValue };

        // Act
        var response = await client.PatchAsync("api/paper/discontinue", JsonContent.Create(mixedPaperIds));

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var updatedPaper = await _pgCtxSetup.DbContextInstance.Papers.FindAsync(validPaperId);
        Assert.NotNull(updatedPaper);
        Assert.True(updatedPaper.Discontinued);
    }
    
    [Fact]
    public async Task DiscontinueBulk_DuplicatePaperIds_ReturnsNoContent()
    {
        // Arrange
        var client = CreateClient();

        var createPaperModel = new PaperCreateModel
        {
            Name = "Paper with Duplicates",
            Price = 8.99,
            Stock = 120,
            PropertyIds = []
        };

        var createPaperResponse = await client.PostAsJsonAsync("api/paper", new List<PaperCreateModel> { createPaperModel });
        Assert.Equal(HttpStatusCode.Created, createPaperResponse.StatusCode);

        var createdPaper = await createPaperResponse.Content.ReadFromJsonAsync<List<PaperDetailViewModel>>();
        Assert.NotNull(createdPaper);
        var validPaperId = createdPaper.First().Id;

        var duplicatePaperIds = new List<int> { validPaperId, validPaperId };

        // Act
        var response = await client.PatchAsync("api/paper/discontinue", JsonContent.Create(duplicatePaperIds));

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var updatedPaper = await _pgCtxSetup.DbContextInstance.Papers.FindAsync(validPaperId);
        Assert.NotNull(updatedPaper);
        Assert.True(updatedPaper.Discontinued);
    }
    
    [Fact]
    public async Task Restock_ValidPaperIdAndAmount_ReturnsNoContent()
    {
        // Arrange
        var client = CreateClient();
        var createPaperModel = new PaperCreateModel
        {
            Name = "Test Paper",
            Price = 10.99,
            Stock = 100,
            PropertyIds = []
        };

        var createPaperResponse = await client.PostAsJsonAsync("api/paper", new List<PaperCreateModel> { createPaperModel });
        Assert.Equal(HttpStatusCode.Created, createPaperResponse.StatusCode);

        var paperData = await createPaperResponse.Content.ReadFromJsonAsync<List<PaperDetailViewModel>>();
        Assert.NotNull(paperData);
        var validPaperId = paperData.First().Id;

        const int restockAmount = 50;

        // Act
        var response = await client.PatchAsync($"api/paper/{validPaperId}/restock?amount={restockAmount}", null);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var updatedPaper = await _pgCtxSetup.DbContextInstance.Papers.FindAsync(validPaperId);
        Assert.NotNull(updatedPaper);
        Assert.Equal(150, updatedPaper.Stock);
    }
    
    [Fact]
    public async Task Restock_InvalidPaperId_ReturnsBadRequest()
    {
        // Arrange
        var client = CreateClient();
        const int invalidPaperId = -1;
        const int restockAmount = 50;

        // Act
        var response = await client.PatchAsync($"api/paper/{invalidPaperId}/restock?amount={restockAmount}", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task Restock_NonExistentPaperId_ReturnsNotFound()
    {
        // Arrange
        var client = CreateClient();
        const int nonExistentPaperId = int.MaxValue;
        const int restockAmount = 50;

        // Act
        var response = await client.PatchAsync($"api/paper/{nonExistentPaperId}/restock?amount={restockAmount}", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task Restock_DiscontinuedPaperId_ReturnsNotFound()
    {
        // Arrange
        var client = CreateClient();
        var createPaperModel = new PaperCreateModel
        {
            Name = "Discontinued Paper",
            Price = 12.99,
            Stock = 50,
            PropertyIds = []
        };

        var createPaperResponse = await client.PostAsJsonAsync("api/paper", new List<PaperCreateModel> { createPaperModel });
        Assert.Equal(HttpStatusCode.Created, createPaperResponse.StatusCode);

        var paperData = await createPaperResponse.Content.ReadFromJsonAsync<List<PaperDetailViewModel>>();
        Assert.NotNull(paperData);
        var paperId = paperData.First().Id;

        var discontinueResponse = await client.PatchAsync($"api/paper/{paperId}/discontinue", null);
        Assert.Equal(HttpStatusCode.NoContent, discontinueResponse.StatusCode);

        const int restockAmount = 50;

        // Act
        var response = await client.PatchAsync($"api/paper/{paperId}/restock?amount={restockAmount}", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task Restock_InvalidRestockAmount_ReturnsBadRequest()
    {
        // Arrange
        var client = CreateClient();
        var createPaperModel = new PaperCreateModel
        {
            Name = "Test Paper",
            Price = 10.99,
            Stock = 100,
            PropertyIds = []
        };

        var createPaperResponse = await client.PostAsJsonAsync("api/paper", new List<PaperCreateModel> { createPaperModel });
        Assert.Equal(HttpStatusCode.Created, createPaperResponse.StatusCode);

        var paperData = await createPaperResponse.Content.ReadFromJsonAsync<List<PaperDetailViewModel>>();
        Assert.NotNull(paperData);
        var validPaperId = paperData.First().Id;

        const int invalidRestockAmount = -50;

        // Act
        var response = await client.PatchAsync($"api/paper/{validPaperId}/restock?amount={invalidRestockAmount}", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task Restock_StockShouldNotExceedIntMaxValue()
    {
        // Arrange
        var client = CreateClient();
        var createPaperModel = new PaperCreateModel
        {
            Name = "Test Paper for MaxValue",
            Price = 15.99,
            Stock = int.MaxValue - 10,
            PropertyIds = []
        };

        var createPaperResponse = await client.PostAsJsonAsync("api/paper", new List<PaperCreateModel> { createPaperModel });
        Assert.Equal(HttpStatusCode.Created, createPaperResponse.StatusCode);

        var paperData = await createPaperResponse.Content.ReadFromJsonAsync<List<PaperDetailViewModel>>();
        Assert.NotNull(paperData);
        var validPaperId = paperData.First().Id;

        const int restockAmount = 50;

        // Act
        var response = await client.PatchAsync($"api/paper/{validPaperId}/restock?amount={restockAmount}", null);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var updatedPaper = await _pgCtxSetup.DbContextInstance.Papers.FindAsync(validPaperId);
        Assert.NotNull(updatedPaper);
        Assert.Equal(int.MaxValue, updatedPaper.Stock);
    }

    [Fact]
    public async Task RestockBulk_ValidPaperIdsAndAmounts_ReturnsNoContent()
    {
        // Arrange
        var client = CreateClient();

        var papersToCreate = new List<PaperCreateModel>
        {
            new() { Name = "Paper 1", Price = 5.99, Stock = 100, PropertyIds = [] },
            new() { Name = "Paper 2", Price = 7.99, Stock = 200, PropertyIds = [] }
        };

        var createPapersResponse = await client.PostAsJsonAsync("api/paper", papersToCreate);
        Assert.Equal(HttpStatusCode.Created, createPapersResponse.StatusCode);

        var createdPapers = await createPapersResponse.Content.ReadFromJsonAsync<List<PaperDetailViewModel>>();
        Assert.NotNull(createdPapers);

        var restockRequests = createdPapers.Select(p => new PaperRestockUpdateModel { PaperId = p.Id, Amount = 50 }).ToList();

        // Act
        var response = await client.PatchAsync("api/paper/restock", JsonContent.Create(restockRequests));

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        foreach (var request in restockRequests)
        {
            var updatedPaper = await _pgCtxSetup.DbContextInstance.Papers.FindAsync(request.PaperId);
            Assert.NotNull(updatedPaper);

            var expectedStock = createdPapers.First(p => p.Id == request.PaperId).Stock + request.Amount;
            Assert.Equal(expectedStock, updatedPaper.Stock);
        }
    }
    
    [Fact]
    public async Task RestockBulk_InvalidPaperIds_ReturnsBadRequest()
    {
        // Arrange
        var client = CreateClient();
        var invalidRestockRequests = new List<PaperRestockUpdateModel>
        {
            new() { PaperId = -1, Amount = 50 },
            new() { PaperId = 0, Amount = 30 }
        };

        // Act
        var response = await client.PatchAsync("api/paper/restock", JsonContent.Create(invalidRestockRequests));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task RestockBulk_NoValidPaperIds_ReturnsNotFound()
    {
        // Arrange
        var client = CreateClient();
        var nonExistentRestockRequests = new List<PaperRestockUpdateModel>
        {
            new() { PaperId = int.MaxValue, Amount = 50 },
            new() { PaperId = int.MaxValue - 1, Amount = 30 }
        };

        // Act
        var response = await client.PatchAsync("api/paper/restock", JsonContent.Create(nonExistentRestockRequests));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task RestockBulk_DiscontinuedPaperId_ReturnsNotFound()
    {
        // Arrange
        var client = CreateClient();
        var createPaperModel = new PaperCreateModel
        {
            Name = "Discontinued Paper",
            Price = 12.99,
            Stock = 50,
            PropertyIds = []
        };

        var createPaperResponse = await client.PostAsJsonAsync("api/paper", new List<PaperCreateModel> { createPaperModel });
        Assert.Equal(HttpStatusCode.Created, createPaperResponse.StatusCode);

        var paperData = await createPaperResponse.Content.ReadFromJsonAsync<List<PaperDetailViewModel>>();
        Assert.NotNull(paperData);
        var paperId = paperData.First().Id;

        var discontinueResponse = await client.PatchAsync($"api/paper/{paperId}/discontinue", null);
        Assert.Equal(HttpStatusCode.NoContent, discontinueResponse.StatusCode);

        var restockRequests = new List<PaperRestockUpdateModel>
        {
            new() { PaperId = paperId, Amount = 50 }
        };

        // Act
        var response = await client.PatchAsync("api/paper/restock", JsonContent.Create(restockRequests));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task RestockBulk_DuplicatePaperIds_ReturnsBadRequest()
    {
        // Arrange
        var client = CreateClient();

        var createPaperModel = new PaperCreateModel
        {
            Name = "Duplicate Paper",
            Price = 8.99,
            Stock = 120,
            PropertyIds = []
        };

        var createPaperResponse = await client.PostAsJsonAsync("api/paper", new List<PaperCreateModel> { createPaperModel });
        Assert.Equal(HttpStatusCode.Created, createPaperResponse.StatusCode);

        var paperData = await createPaperResponse.Content.ReadFromJsonAsync<List<PaperDetailViewModel>>();
        Assert.NotNull(paperData);
        var validPaperId = paperData.First().Id;

        var duplicateRestockRequests = new List<PaperRestockUpdateModel>
        {
            new() { PaperId = validPaperId, Amount = 50 },
            new() { PaperId = validPaperId, Amount = 30 }
        };

        // Act
        var response = await client.PatchAsync("api/paper/restock", JsonContent.Create(duplicateRestockRequests));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task CreatePapers_ValidPaperList_ReturnsCreated()
    {
        // Arrange
        var client = CreateClient();

        var papersToCreate = new List<PaperCreateModel>
        {
            new() { Name = "Paper 1", Price = 5.99, Stock = 100, PropertyIds = [] },
            new() { Name = "Paper 2", Price = 7.99, Stock = 200, PropertyIds = [] }
        };

        // Act
        var response = await client.PostAsJsonAsync("api/paper", papersToCreate);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdPapers = await response.Content.ReadFromJsonAsync<List<PaperDetailViewModel>>();
        Assert.NotNull(createdPapers);
        Assert.Equal(papersToCreate.Count, createdPapers.Count);

        foreach (var paper in papersToCreate)
        {
            Assert.Contains(createdPapers, p => p.Name == paper.Name && Math.Abs(p.Price - paper.Price) < 0.0001 && p.Stock == paper.Stock);
        }
    }
    
    [Fact]
    public async Task CreatePapers_DuplicatePaperNames_ReturnsBadRequest()
    {
        // Arrange
        var client = CreateClient();

        var papersToCreate = new List<PaperCreateModel>
        {
            new() { Name = "Duplicate Paper", Price = 5.99, Stock = 100, PropertyIds = [] },
            new() { Name = "Duplicate Paper", Price = 7.99, Stock = 200, PropertyIds = [] }
        };

        // Act
        var response = await client.PostAsJsonAsync("api/paper", papersToCreate);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task CreatePapers_ExistingPaperNames_ReturnsConflict()
    {
        // Arrange
        var client = CreateClient();

        var existingPaper = new Paper
        {
            Name = "Existing Paper",
            Price = 5.99,
            Stock = 100,
            Discontinued = false
        };

        await _pgCtxSetup.DbContextInstance.Papers.AddAsync(existingPaper);
        await _pgCtxSetup.DbContextInstance.SaveChangesAsync();

        var papersToCreate = new List<PaperCreateModel>
        {
            new() { Name = "Existing Paper", Price = 7.99, Stock = 200, PropertyIds = [] }
        };

        // Act
        var response = await client.PostAsJsonAsync("api/paper", papersToCreate);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
    
    [Fact]
    public async Task CreatePapers_WithProperties_ReturnsCreated()
    {
        // Arrange
        var client = CreateClient();

        // Create properties
        var createPropertyModel1 = new PaperPropertyCreateModel { Name = "Recycled" };
        var createPropertyModel2 = new PaperPropertyCreateModel { Name = "Glossy" };

        var propertyResponse1 = await client.PostAsJsonAsync("api/paper/property", createPropertyModel1);
        var propertyResponse2 = await client.PostAsJsonAsync("api/paper/property", createPropertyModel2);

        Assert.Equal(HttpStatusCode.Created, propertyResponse1.StatusCode);
        Assert.Equal(HttpStatusCode.Created, propertyResponse2.StatusCode);

        var propertyData1 = await propertyResponse1.Content.ReadFromJsonAsync<PaperPropertyDetailViewModel>();
        var propertyData2 = await propertyResponse2.Content.ReadFromJsonAsync<PaperPropertyDetailViewModel>();

        Assert.NotNull(propertyData1);
        Assert.NotNull(propertyData2);

        var papersToCreate = new List<PaperCreateModel>
        {
            new()
            {
                Name = "Paper with Properties",
                Price = 10.99,
                Stock = 300,
                PropertyIds = new List<int> { propertyData1.Id, propertyData2.Id }
            }
        };

        // Act
        var response = await client.PostAsJsonAsync("api/paper", papersToCreate);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdPapers = await response.Content.ReadFromJsonAsync<List<PaperDetailViewModel>>();
        Assert.NotNull(createdPapers);
        var createdPaper = createdPapers.First();
        Assert.Equal("Paper with Properties", createdPaper.Name);
        Assert.Equal(2, createdPaper.Properties.Count);
        Assert.Contains(createdPaper.Properties, p => p.Id == propertyData1.Id);
        Assert.Contains(createdPaper.Properties, p => p.Id == propertyData2.Id);
    }
    
    private class PaperIdComparer : IEqualityComparer<PaperDetailViewModel>
    {
        public bool Equals(PaperDetailViewModel x, PaperDetailViewModel y) => x.Id == y.Id;
        public int GetHashCode(PaperDetailViewModel obj) => obj.Id.GetHashCode();
    }
    
    private class PaperPriceComparer : IEqualityComparer<PaperDetailViewModel>
    {
        public bool Equals(PaperDetailViewModel x, PaperDetailViewModel y) => Math.Abs(x.Price - y.Price) < 0.0001;
        public int GetHashCode(PaperDetailViewModel obj) => obj.Price.GetHashCode();
    }

    private class PaperNameComparer : IEqualityComparer<PaperDetailViewModel>
    {
        public bool Equals(PaperDetailViewModel x, PaperDetailViewModel y) => string.Equals(x.Name, y.Name, StringComparison.Ordinal);
        public int GetHashCode(PaperDetailViewModel obj) => obj.Name.GetHashCode();
    }

    private class PaperStockComparer : IEqualityComparer<PaperDetailViewModel>
    {
        public bool Equals(PaperDetailViewModel x, PaperDetailViewModel y) => x.Stock == y.Stock;
        public int GetHashCode(PaperDetailViewModel obj) => obj.Stock.GetHashCode();
    }
}