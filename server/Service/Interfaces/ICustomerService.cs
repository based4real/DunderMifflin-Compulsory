using DataAccess.Models;
using Service.TransferModels;

namespace Service.Interfaces;

public interface ICustomerService
{
    public Task<List<CustomerDetailViewModel>> All();
    
    public Task<CustomerDetailViewModel?> ById(int id);
}