using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Models.Requests;
using Service.Models.Responses;
using SharedDependencies.Enums;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaperController(IPaperService service) : ControllerBase
{
    /// <summary>
    /// Creates a list of new paper products.
    /// </summary>
    /// <param name="papers">A list of paper products to create, including details like name, stock, price, and properties.</param>
    /// <returns>A list of created paper products with their details.</returns>
    /// <response code="201">Paper products created successfully.</response>
    /// <response code="400">Provided paper names contain duplicates.</response>
    /// <response code="409">Paper products with the same name already exist.</response>
    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(List<PaperDetailViewModel>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<List<PaperDetailViewModel>>> CreatePapers([FromBody] List<PaperCreateModel> papers)
    {
        return Created("", await service.CreatePapers(papers));
    }

    /// <summary>
    /// Creates a Paper Property.
    /// </summary>
    /// <param name="property">The property details</param>
    /// <returns>The created property.</returns>
    /// <response code="201">Property created successfully.</response>
    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(PaperPropertyDetailViewModel), StatusCodes.Status201Created)]
    [Route("property")]
    public async Task<ActionResult<PaperPropertyDetailViewModel>> CreatePaperProperty([FromBody] PaperPropertyCreateModel property)
    {
        var createdProperty = await service.CreateProperty(property);
        return CreatedAtAction(nameof(CreatePaperProperty), new { id = createdProperty.Id }, createdProperty);
    }

    /// <summary>
    /// Retrieves a paginated list of paper products.
    /// </summary>
    /// <param name="page">Page number to retrieve, must be greater than 0. Default is 1.</param>
    /// <param name="pageSize">Number of items per page, between 1 and 1000. Default is 10.</param>
    /// <param name="search">Optional search term for paper names (case-insensitive).</param>
    /// <param name="discontinued">Filter by discontinued status. Null returns all papers.</param>
    /// <param name="orderBy">Field to order results by, default is <see cref="PaperOrderBy.Id"/>.</param>
    /// <param name="sortBy">Sorting direction: ascending or descending. Default is <see cref="SortOrder.Asc"/>.</param>
    /// <param name="filter">Comma-separated property IDs to filter by.</param>
    /// <param name="filterType">Specifies whether all (<see cref="FilterType.And"/>) or any (<see cref="FilterType.Or"/>) properties should be matched. Default is <see cref="FilterType.Or"/>.</param>
    /// <param name="minPrice">Optional minimum price to filter products. Null returns all products regardless of minimum price.</param>
    /// <param name="maxPrice">Optional maximum price to filter products. Null returns all products regardless of maximum price.</param>
    /// <returns>Paginated list of paper products matching the criteria.</returns>
    /// <response code="200">Returns the paginated list of paper products.</response>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(PaperPagedViewModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaperPagedViewModel>> All(
        [FromQuery, Range(1, int.MaxValue)] int page = 1,
        [FromQuery, Range(1, 1000)] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] bool? discontinued = null,
        [FromQuery] PaperOrderBy orderBy = PaperOrderBy.Id,
        [FromQuery] SortOrder sortBy = SortOrder.Asc,
        [FromQuery] string? filter = null,
        [FromQuery] FilterType filterType = FilterType.Or,
        [FromQuery] double? minPrice = null,
        [FromQuery] double? maxPrice = null)
    {
        var propertyIds = filter?.Split(',').Select(int.Parse).ToList();
        
        return Ok(await service.AllPaged(page, pageSize, search, discontinued, orderBy, sortBy, propertyIds, filterType, minPrice, maxPrice));
    }
    
    /// <summary>
    /// Sets a paper product as discontinued.
    /// </summary>
    /// <param name="id">The ID of the paper to discontinue, must be a positive integer.</param>
    /// <response code="204">The paper was successfully discontinued.</response>
    /// <response code="400">Invalid paper ID provided.</response>
    /// <response code="404">Paper with the provided ID not found.</response>
    [HttpPatch("{id}/discontinue")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Discontinue([Range(1, int.MaxValue)] int id)
    {
        await service.Discontinue([id]);
        return NoContent();
    }
    
    /// <summary>
    /// Sets multiple paper products as discontinued.
    /// </summary>
    /// <param name="ids">List of paper IDs to discontinue, must be positive integers with at least one element.</param>
    /// <response code="204">Paper products were successfully discontinued.</response>
    /// <response code="400">Invalid or empty list of IDs provided.</response>
    /// <response code="404">None of the provided IDs match existing papers.</response>
    /// <remarks>
    /// - Duplicate IDs are removed.
    /// - Negative or zero IDs are ignored.
    /// - If no valid IDs remain after filtering, an error is returned.
    /// - Valid IDs are processed, and invalid IDs are disregarded.
    /// </remarks>
    [HttpPatch("discontinue")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DiscontinueBulk([FromBody, MinLength(1)] List<int> ids)
    {
        await service.Discontinue(ids);
        return NoContent();
    }
    
    /// <summary>
    /// Restocks a paper product.
    /// </summary>
    /// <param name="id">The ID of the paper to restock, must be a positive integer.</param>
    /// <param name="amount">Amount to add to the current stock, must be greater than 0.</param>
    /// <response code="204">The paper was successfully restocked.</response>
    /// <response code="400">Invalid ID or amount provided.</response>
    /// <response code="404">Paper not found or discontinued.</response>
    [HttpPatch("{id}/restock")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Restock([Range(1, int.MaxValue)] int id,
                                             [FromQuery, Required, Range(1, int.MaxValue)] int amount)
    {
        await service.Restock([new PaperRestockUpdateModel { PaperId = id, Amount = amount }]);
        return NoContent();
    }
    
    /// <summary>
    /// Restocks multiple paper products.
    /// </summary>
    /// <param name="restockRequests">List of paper restock requests, each containing a paper ID and an amount to add to stock.</param>
    /// <response code="204">Paper products were successfully restocked.</response>
    /// <response code="400">Invalid list of requests or amounts provided.</response>
    /// <response code="404">None of the provided IDs match existing papers or refer to discontinued products.</response>
    /// <remarks>
    /// - Duplicate paper IDs are not allowed.
    /// - Negative or zero IDs and amounts are invalid.
    /// - Papers that are discontinued cannot be restocked.
    /// - Valid requests are processed, invalid ones are disregarded.
    /// </remarks>
    [HttpPatch("restock")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestockBulk([FromBody, MinLength(1)] List<PaperRestockUpdateModel> restockRequests)
    {
        await service.Restock(restockRequests);
        return NoContent();
    }
    
    /// <summary>
    /// Retrieves a list of all paper properties.
    /// </summary>
    /// <returns>List of paper properties.</returns>
    /// <response code="200">Returns the list of paper properties.</response>
    [HttpGet("property")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(List<PaperPropertySummaryViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PaperPropertySummaryViewModel>>> AllProperties()
    {
        return Ok(await service.AllProperties());
    }
}