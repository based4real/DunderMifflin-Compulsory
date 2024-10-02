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

    [HttpGet]
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
    
    /// <response code="404">If the paper is not found</response>
    [HttpPatch("{id}/discontinue")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Discontinue(int id)
    {
        await service.Discontinue(id);
        return NoContent();
    }
    
    /// <summary>
    /// Restocks a paper product by adding a specified amount to the current stock.
    /// </summary>
    /// <param name="id">The ID of the paper to restock.</param>
    /// <param name="amount">The amount to add to the current stock. Must be a positive amount.</param>
    /// <response code="204">If the restocking is successful.</response>
    /// <response code="404">If the paper is not found.</response>
    [HttpPatch("{id}/restock")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Restock(int id, [FromQuery, Range(1, int.MaxValue)] int amount)
    {
        await service.Restock(id, amount);
        return NoContent();
    }
}