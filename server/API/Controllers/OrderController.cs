using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Models.Order;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController(IOrderService service) : ControllerBase
{
    public async Task<ActionResult<OrderDetailViewModel>> CreateOrder([FromBody] OrderCreateModel order)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await service.Create(order);
        if (created == null)
            return BadRequest("An error occured");

        return created;
    }
}