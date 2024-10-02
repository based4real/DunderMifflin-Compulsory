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
    /// <param name="papers">A list of paper products to create, each containing details like name, stock, price, and properties.</param>
    /// <returns>A list of created paper products with their details.</returns>
    /// <response code="201">Paper products created successfully.</response>
    /// <response code="400">One or more provided paper names are duplicated.</response>
    /// <response code="409">One or more paper products with the same name already exist.</response>
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
    /// <param name="page">The page number to retrieve. Must be greater than 0. Default is 1.</param>
    /// <param name="pageSize">The number of items per page. Must be between 1 and 1000. Default is 10.</param>
    /// <param name="search">A string to search for in the paper names. If provided, results are filtered to contain this string (case-insensitive).</param>
    /// <param name="discontinued">Whether to filter papers by their discontinued status. Null returns both discontinued and non-discontinued papers.</param>
    /// <param name="orderBy">The field by which to order the results. Defaults to <see cref="PaperOrderBy.Id"/>.</param>
    /// <param name="sortBy">The sorting direction: ascending or descending. Defaults to <see cref="SortOrder.Asc"/>.</param>
    /// <param name="filter">Comma-separated list of property IDs to filter papers by. Only papers with these properties will be included.</param>
    /// <param name="filterType">Specifies whether all (<see cref="FilterType.And"/>) or any (<see cref="FilterType.Or"/>) of the provided properties should be matched. Defaults to <see cref="FilterType.Or"/>.</param>
    /// <returns>A paginated view model containing the list of paper products that match the provided criteria.</returns>
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
        [FromQuery] FilterType filterType = FilterType.Or)
    {
        var propertyIds = filter?.Split(',').Select(int.Parse).ToList();
        
        return Ok(await service.AllPaged(page, pageSize, search, discontinued, orderBy, sortBy, propertyIds, filterType));
    }
    
    /// <summary>
    /// Marks a paper product as discontinued by setting its discontinued status to true.
    /// </summary>
    /// <param name="id">The ID of the paper to discontinue. Must be a positive integer greater than 0.</param>
    /// <response code="204">The paper was successfully discontinued.</response>
    /// <response code="400">If the provided ID is invalid (e.g., less than or equal to 0).</response>
    /// <response code="404">If the paper with the provided ID is not found.</response>
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
    /// Marks multiple paper products as discontinued by setting their discontinued status to true.
    /// </summary>
    /// <param name="ids">A list of IDs of the papers to discontinue. Must be a list of positive integers greater than 0, with at least one element.</param>
    /// <response code="204">The specified paper products were successfully discontinued.</response>
    /// <response code="400">If the provided list of IDs is empty, contains only invalid IDs (e.g., negative numbers or zeros), or contains duplicate IDs.</response>
    /// <response code="404">If none of the provided IDs correspond to existing papers.</response>
    /// <remarks>
    /// The following actions are performed by this endpoint:
    /// - Duplicate IDs in the request are automatically removed.
    /// - Any IDs that are 0 or negative are removed.
    /// - If no valid IDs are provided after filtering, an error is returned.
    /// - If some valid IDs are found, those papers are discontinued, and any invalid IDs are disregarded.
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
    /// Restocks a paper product by adding a specified amount to the current stock.
    /// </summary>
    /// <param name="id">The ID of the paper to restock. Must be a positive integer greater than 0.</param>
    /// <param name="amount">The amount to add to the current stock. Must be a positive number greater than 0.</param>
    /// <response code="204">The paper was successfully restocked.</response>
    /// <response code="400">If the provided ID or amount is invalid.</response>
    /// <response code="404">If the paper with the provided ID is not found or if product was discontinued.</response>
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
    /// Restocks multiple paper products by adding specified amounts to their current stock.
    /// </summary>
    /// <param name="restockRequests">A list of paper restock requests, each containing a paper ID and the amount to add to its stock.</param>
    /// <response code="204">The specified paper products were successfully restocked.</response>
    /// <response code="400">If the provided list of IDs is empty, contains only invalid IDs (e.g., negative numbers or zeros), contains duplicate IDs, or if any amounts are invalid.</response>
    /// <response code="404">If none of the provided IDs correspond to existing papers, or if they correspond to discontinued products.</response>
    /// <remarks>
    /// The following actions are performed by this endpoint:
    /// - Duplicate paper IDs in the request are not allowed. If duplicates are found, an error is returned.
    /// - Any IDs that are 0 or negative are considered invalid and will result in a `400 Bad Request`.
    /// - If no valid IDs are provided after filtering, a `404 Not Found` error is returned.
    /// - Papers that are discontinued cannot be restocked, and an error will be returned if they are included in the request.
    /// - If a mix of valid and invalid/discontinued IDs is provided, only valid papers will be restocked, and the rest will be ignored.
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
}