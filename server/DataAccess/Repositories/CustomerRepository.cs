using DataAccess.Interfaces;
using DataAccess.Models;

namespace DataAccess.Repositories;

public class CustomerRepository(AppDbContext context) : ICustomerRepository
{
    public List<Customer> All()
    {
        return context.Customers.ToList();
    }
}