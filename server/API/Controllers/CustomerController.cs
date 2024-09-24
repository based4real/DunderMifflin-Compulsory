using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController(ICustomerService service) : ControllerBase
{
    [HttpGet]
    public ActionResult<List<Customer>> All()
    {
        return Ok(service.All());
    }

    [HttpGet]
    [Route("{id}")]
    public ActionResult<Customer> Get(int id)
    {
        var customer = service.ById(id);
        if (customer == null)
            return NotFound();
        
        return Ok(customer);
    }
}