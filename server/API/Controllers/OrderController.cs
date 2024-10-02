using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Models.Requests;
using Service.Models.Responses;
using SharedDependencies.Enums;

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
    /// <response code="201">Order created successfully.</response>
    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(OrderDetailViewModel), StatusCodes.Status201Created)]
    public async Task<ActionResult<OrderDetailViewModel>> CreateOrder([FromBody] OrderCreateModel order)
    {
        var createdOrder = await service.Create(order);
        return CreatedAtAction(nameof(CreateOrder), new { id = createdOrder.Id }, createdOrder);
    }
    
    /// <summary>
    /// Updates the status of a specific order.
    /// </summary>
    /// <param name="id">The ID of the order to update, must be a positive integer.</param>
    /// <param name="status">The new status to set, must be a valid `OrderStatus` value.</param>
    /// <response code="204">The order status was successfully updated.</response>
    /// <response code="400">Invalid order ID or status value.</response>
    /// <response code="404">The order with the provided ID was not found.</response>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOrderStatus(int id, OrderStatus status)
    {
        await service.UpdateOrderStatus([id], status);
        return NoContent();
    }
    
    /// <summary>
    /// Updates the status of multiple orders.
    /// </summary>
    /// <param name="ids">A list of order IDs to update, must be positive integers.</param>
    /// <param name="status">The new status to set, must be a valid `OrderStatus` value.</param>
    /// <response code="204">The order statuses were successfully updated.</response>
    /// <response code="400">Invalid list of IDs or status value.</response>
    /// <response code="404">None of the provided IDs match existing orders.</response>
    /// <remarks>
    /// - Duplicate IDs are removed.
    /// - Negative or zero IDs are rejected with `400 Bad Request`.
    /// - If no valid IDs remain, `404 Not Found` is returned.
    /// - If you provide a mix of valid and invalid IDs, the status will be updated only for the valid orders.
    /// </remarks>
    [HttpPatch("status")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOrderStatusBulk([FromBody, MinLength(1)] List<int> ids, [FromQuery] OrderStatus status)
    {
        await service.UpdateOrderStatus(ids, status);
        return NoContent();
    }
}