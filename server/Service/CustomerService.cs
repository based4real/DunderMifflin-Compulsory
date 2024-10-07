using DataAccess;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Service.Exceptions;
using Service.Interfaces;
using Service.Models;
using Service.Models.Responses;

namespace Service;

public class CustomerService(AppDbContext context) : ICustomerService
{
    public async Task<CustomerPagedViewModel> All(int pageNumber, int itemsPerPage, bool includeOrderHistory)
    {
        IQueryable<Customer> query = context.Customers;

        if (includeOrderHistory)
        {
            query = query
                .Where(o => o.Orders.Count > 0)
                .Include(c => c.Orders)
                .ThenInclude(d => d.OrderEntries)
                .ThenInclude(e => e.Product);
        }
        else
        {
            query = query.Include(c => c.Orders);
        }

        var totalItems = await query.CountAsync();

        var pagedCustomers = await query
            .Skip((pageNumber - 1) * itemsPerPage)
            .Take(itemsPerPage)
            .Select(customer => CustomerOrderDetailViewModel.FromEntity(customer, includeOrderHistory))
            .ToListAsync();

        if (includeOrderHistory && pagedCustomers.Count == 0)
            throw new NotFoundException("No customers with order history found");

        return new CustomerPagedViewModel
        {
            Customers = pagedCustomers,
            PagingInfo = new PagingInfo
            {
                TotalItems = totalItems,
                ItemsPerPage = itemsPerPage,
                CurrentPage = pageNumber
            }
        };
    }
    
    public async Task<CustomerDetailViewModel?> ById(int id)
    {
        var result = await context.Customers
            .Include(customer => customer.Orders)
            .Where(customer => customer.Id == id)
            .Select(customer => CustomerDetailViewModel.FromEntity(customer))
            .SingleOrDefaultAsync();

        if (result == null)
            throw new NotFoundException($"Customer with ID {id} not found");

        return result;
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

    public async Task<OrderDetailViewModel> CustomerOrder(int customerId, int orderId)
    {
        var customer = await context.Customers
            .Include(customer => customer.Orders)
            .ThenInclude(order => order.OrderEntries)
            .ThenInclude(entry => entry.Product)
            .SingleOrDefaultAsync(customer => customer.Id == customerId);
        
        if (customer == null)
            throw new NotFoundException($"Customer with id {customerId} not found");
            
        var order = customer.Orders.SingleOrDefault(order => order.Id == orderId);
        
        if (order == null)
            throw new NotFoundException($"Order with id {orderId} for customer with id {customerId} not found");
            
        return OrderDetailViewModel.FromEntity(order);
    }
}