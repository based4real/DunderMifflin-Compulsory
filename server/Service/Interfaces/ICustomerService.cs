using DataAccess.Models;

namespace Service.Interfaces;

public interface ICustomerService
{
    public List<Customer> All();
    
    public Customer ById(int id);
}