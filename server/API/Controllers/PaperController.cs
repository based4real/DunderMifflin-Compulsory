using Microsoft.AspNetCore.Mvc;
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
}