using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Models.Responses;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController(ICustomerService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CustomerDetailViewModel>>> All()
    {
        return Ok(await service.All());
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<CustomerDetailViewModel?>> Get([Range(1, int.MaxValue)] int id)
    {
        return Ok(await service.ById(id));
    }
    
    
    [HttpGet]
    [Route("/all")]
    public async Task<ActionResult<List<CustomerOrderDetailViewModel>>> AllOrderHistory()
    {
        return Ok(await service.GetOrderHistoryForAll());
    }
}