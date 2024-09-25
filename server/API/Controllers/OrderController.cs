using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Models.Requests;
using Service.Models.Responses;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController(IOrderService service) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<OrderDetailViewModel>> CreateOrder([FromBody] OrderCreateModel order)
    {
        return Ok(await service.Create(order));
    }
}