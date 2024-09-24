using DataAccess;
using Microsoft.EntityFrameworkCore;
using Service.Interfaces;
using Service.TransferModels;

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
        return await context.Customers
            .Include(customer => customer.Orders)
            .Where(customer => customer.Id == id)
            .Select(customer => CustomerDetailViewModel.FromEntity(customer))
            .SingleOrDefaultAsync();
    }
}