using Service.Models.Responses;

namespace Service.Interfaces;

public interface ICustomerService
{
    public Task<List<CustomerDetailViewModel>> All();
    
    public Task<CustomerDetailViewModel?> ById(int id);
    
    public Task<List<CustomerOrderDetailViewModel>> AllWithOrderHistory();
    
    public Task<CustomerOrderPagedViewModel> GetPagedOrdersForCustomer(int customerId, int pageNumber, int itemsPerPage);
    
    public Task<OrderDetailViewModel> CustomerOrder(int customerId, int orderId);
}