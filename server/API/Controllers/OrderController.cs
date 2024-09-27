using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Models.Requests;
using Service.Models.Responses;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController(IOrderService service) : ControllerBase
{
    /// <summary>
    /// Creates a new order.
    /// </summary>
    /// <param name="order">The order details</param>
    /// <returns>The created order.</returns>
    /// <response code="200">Order created successfully.</response>
    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(OrderDetailViewModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<OrderDetailViewModel>> CreateOrder([FromBody] OrderCreateModel order)
    {
        return Ok(await service.Create(order));
    }
}