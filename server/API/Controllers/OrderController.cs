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
    /// <param name="id">he ID of the order to update. Must be a positive integer greater than 0.</param>
    /// <param name="status">The new status to set for the order. Must be a valid `OrderStatus` value.</param>
    /// <response code="204">The order status was successfully updated.</response>
    /// <response code="400">If the provided ID or status is invalid.</response>
    /// <response code="404">If the order with the provided ID is not found.</response>
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
    /// <param name="ids">A list of order IDs to update, each must be a positive integer greater than 0.</param>
    /// <param name="status">The new status to set for the orders. Must be a valid `OrderStatus` value.</param>
    /// <response code="204">The order statuses were successfully updated.</response>
    /// <response code="400">If the provided list of IDs is empty, contains only invalid IDs, or if the status value is invalid.</response>
    /// <response code="404">If none of the provided IDs correspond to existing orders.</response>
    /// <remarks>
    /// The following actions are performed by this endpoint:
    /// - Duplicate order IDs in the request are removed.
    /// - IDs that are 0 or negative are considered invalid and will result in a `400 Bad Request`.
    /// - If no valid IDs are provided after filtering, a `404 Not Found` error is returned.
    /// - If a mix of valid and invalid IDs is provided, only valid orders will have their status updated.
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