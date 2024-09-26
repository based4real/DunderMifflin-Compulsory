using DataAccess;
using Microsoft.EntityFrameworkCore;
using Service.Exceptions;
using Service.Interfaces;
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

    public async Task<List<CustomerOrderDetailViewModel>> GetOrderHistoryForAll()
    {
        return context.Customers
            .Where(o => o.Orders.Count > 0)
            .Include(c => c.Orders)
                .ThenInclude(d => d.OrderEntries)
                    .ThenInclude(e => e.Product)
            .Select(customer => CustomerOrderDetailViewModel.FromEntity(customer)).ToList();
    }
}