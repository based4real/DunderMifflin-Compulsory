using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Models.Responses;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController(ICustomerService service) : ControllerBase
{
    /// <summary>
    /// Retrieve all customers.
    /// </summary>
    /// <param name="orders">Include order history if true.</param>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paged list of customers.</returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CustomerPagedViewModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<CustomerPagedViewModel>> All([FromQuery] bool orders = false,
                                                                [FromQuery, Range(1, int.MaxValue)] int page = 1, 
                                                                [FromQuery, Range(1, 1000)] int pageSize = 10)
    {
        return Ok(await service.All(page, pageSize, orders));
    }

    /// <summary>
    /// Retrieves a customer by ID.
    /// </summary>
    /// <param name="id">The ID of the customer to retrieve.</param>
    /// <returns>The customer details.</returns>
    /// <response code="200">Returns the customer if found.</response>
    /// <response code="404">If the customer is not found.</response>
    [HttpGet("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CustomerDetailViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDetailViewModel?>> Get([Range(1, int.MaxValue)] int id)
    {
        return Ok(await service.ById(id));
    }
    
    /// <summary>
    /// Retrieves a paginated list of orders for a specified customer.
    /// </summary>
    /// <param name="id">The ID of the customer whose orders are being retrieved</param>
    /// <param name="page">The current page (default is 1)</param>
    /// <param name="pageSize">The number of orders per page (default is 10)</param>
    /// <returns>A paginated list of orders for the specified customer</returns>
    /// <response code="200">Returns the customer's order history with pagination</response>
    /// <response code="404">If the customer is not found</response>
    [HttpGet("{id}/Orders")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CustomerOrderPagedViewModel), StatusCodes.Status200OK)] 
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerOrderPagedViewModel>> GetCustomerWithOrders([Range(1, int.MaxValue)] int id,
                                                                                       [FromQuery, Range(1, int.MaxValue)] int page = 1,
                                                                                       [FromQuery, Range(1, 1000)] int pageSize = 10)
    {
        return Ok(await service.GetPagedOrdersForCustomer(id, page, pageSize));
    }
    
    /// <summary>
    /// Retrieves a specific order for a specified customer.
    /// </summary>
    /// <param name="customerId">The ID of the customer whose order is being retrieved</param>
    /// <param name="orderId">The ID of the order to retrieve</param>
    /// <returns>The order details for the specified customer</returns>
    /// <response code="200">Returns the order if found</response>
    /// <response code="404">If the customer or order is not found</response>
    [HttpGet("{customerId}/Orders/{orderId}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(OrderDetailViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDetailViewModel>> GetCustomerOrder([Range(1, int.MaxValue)] int customerId,
                                                                           [Range(1, int.MaxValue)] int orderId)
    {
        return Ok(await service.CustomerOrder(customerId, orderId));
    }
}