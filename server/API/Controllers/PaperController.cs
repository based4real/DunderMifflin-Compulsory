using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Service.Enums;
using Service.Interfaces;
using Service.Models.Requests;
using Service.Models.Responses;

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
        [FromQuery] string? orderBy = null,
        [FromQuery] string? sortBy = null)
    {
        var sortOrder = sortBy ?? "asc";
        return Ok(await service.AllPaged(page, pageSize, search, discontinued, orderBy, sortOrder));
    }
}