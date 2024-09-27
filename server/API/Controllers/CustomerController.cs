using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Interfaces;
using Service.Models.Responses;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController(ICustomerService service) : ControllerBase
{
    /// <summary>
    /// Retrieves all customers.
    /// </summary>
    /// <param name="orders">Include order history if true.</param>
    /// <returns>A list of customer details.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<CustomerDetailViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CustomerDetailViewModel>>> All([FromQuery] bool orders = false)
    {
        if (orders)
        {
            return Ok(await service.AllWithOrderHistory());
        }
        else
        {
            return Ok(await service.All());
        }
    }

    /// <summary>
    /// Retrieves a customer by ID.
    /// </summary>
    /// <param name="id">The ID of the customer to retrieve.</param>
    /// <returns>The customer details.</returns>
    /// <response code="200">Returns the customer if found.</response>
    /// <response code="404">If the customer is not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomerDetailViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDetailViewModel?>> Get([Range(1, int.MaxValue)] int id)
    {
        return Ok(await service.ById(id));
    }
    
    [HttpGet("{id}/Orders")]
    public async Task<ActionResult<CustomerOrderPagedViewModel>> GetCustomerWithOrders(int id, int page = 1, int pageSize = 10)
    {
        return Ok(await service.GetPagedOrdersForCustomer(id, page, pageSize));
    }
}