using DataAccess.Models;

namespace DataAccess.Interfaces;

public interface ICustomerRepository
{
    public List<Customer> All();
}