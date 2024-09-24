using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.TransferModels;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController(ICustomerService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CustomerDetailViewModel>>> All()
    {
        var result = await service.All();
        return Ok(result);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<CustomerDetailViewModel?>> Get(int id)
    {
        var customer = await service.ById(id);
        if (customer == null)
            return NotFound();
        
        return Ok(customer);
    }
}