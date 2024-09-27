using DataAccess.Models;

namespace Service.Models.Responses;

public class CustomerOrderPagedViewModel
{
    public CustomerOrderDetailViewModel CustomerDetails { get; set; } = null!;
    public PagingInfo PagingInfo { get; set; } = new();
    
    public static CustomerOrderPagedViewModel FromEntity(Customer customer, PagingInfo pagingInfo)
    {
        return new CustomerOrderPagedViewModel
        {
            CustomerDetails = CustomerOrderDetailViewModel.FromEntity(customer),
            PagingInfo = pagingInfo
        };
    }
}