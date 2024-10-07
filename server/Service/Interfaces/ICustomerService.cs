using Service.Models.Responses;

namespace Service.Interfaces;

public interface ICustomerService
{
    public Task<CustomerPagedViewModel> All(int pageNumber, int itemsPerPage, bool includeOrderHistory);
    
    public Task<CustomerDetailViewModel?> ById(int id);
    
    public Task<CustomerOrderPagedViewModel> GetPagedOrdersForCustomer(int customerId, int pageNumber, int itemsPerPage);
    
    public Task<OrderDetailViewModel> CustomerOrder(int customerId, int orderId);
}