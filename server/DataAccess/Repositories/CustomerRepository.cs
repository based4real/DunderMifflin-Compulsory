using DataAccess.Interfaces;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class CustomerRepository(AppDbContext context) : ICustomerRepository
{
    public List<Customer> All()
    {
        return context.Customers.ToList();
    }

    public Customer ById(int id)
    {
        return context.Customers.Find(id);
    }
}