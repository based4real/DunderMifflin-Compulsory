using DataAccess;
using Microsoft.EntityFrameworkCore;
using Service.Exceptions;
using Service.Interfaces;
using Service.Models;
using Service.Models.Responses;

namespace Service;

public class CustomerService(AppDbContext context) : ICustomerService
{
    public async Task<List<CustomerDetailViewModel>> All()
    {
        return await context.Customers
            .Include(customer => customer.Orders)
            .Select(customer => CustomerDetailViewModel.FromEntity(customer))
            .ToListAsync();
    }

    public async Task<CustomerDetailViewModel?> ById(int id)
    {
        var result = await context.Customers
            .Include(customer => customer.Orders)
            .Where(customer => customer.Id == id)
            .Select(customer => CustomerDetailViewModel.FromEntity(customer))
            .SingleOrDefaultAsync();

        if (result == null)
            throw new NotFoundException("Customer not found");

        return result;
    }

    public async Task<List<CustomerOrderDetailViewModel>> AllWithOrderHistory()
    {
        var customers = await context.Customers
            .Where(o => o.Orders.Count > 0)
            .Include(c => c.Orders)
            .ThenInclude(d => d.OrderEntries)
            .ThenInclude(e => e.Product)
            .Select(customer => CustomerOrderDetailViewModel.FromEntity(customer)).ToListAsync();

        if (customers.Count == 0)
            throw new NotFoundException("No customers with order history found");

        return customers;
    }

    public async Task<CustomerOrderPagedViewModel> GetPagedOrdersForCustomer(int customerId, int pageNumber, int itemsPerPage)
    {
        var customer = await context.Customers
            .Include(customer => customer.Orders)
            .ThenInclude(order => order.OrderEntries)
            .ThenInclude(entry => entry.Product)
            .SingleOrDefaultAsync(customer => customer.Id == customerId);
            
        if (customer == null)
            throw new NotFoundException($"Customer with id {customerId} not found");
            
        var pagedOrders = customer.Orders.OrderBy(order => order.Id)
            .Skip((pageNumber - 1) * itemsPerPage)
            .Take(itemsPerPage)
            .Select(OrderDetailViewModel.FromEntity)
            .ToList();
            
        return new CustomerOrderPagedViewModel
        {
            CustomerDetails = new CustomerOrderDetailViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                Address = customer.Address,
                Phone = customer.Phone,
                Email = customer.Email,
                Orders = pagedOrders
            },
            PagingInfo = new PagingInfo
            {
                TotalItems = customer.Orders.Count,
                ItemsPerPage = itemsPerPage,
                CurrentPage = pageNumber 
            }
        };
    }
}