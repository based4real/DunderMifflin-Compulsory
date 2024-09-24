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
}