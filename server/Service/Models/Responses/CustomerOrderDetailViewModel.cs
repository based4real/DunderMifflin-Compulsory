using DataAccess.Models;

namespace Service.Models.Responses;

public class CustomerOrderDetailViewModel
{
    public CustomerDetailViewModel? Customer { get; set; }
    public required IEnumerable<OrderDetailViewModel> Orders { get; set; }
    
    public static CustomerOrderDetailViewModel FromEntity(Customer customer)
    {
        return new CustomerOrderDetailViewModel
        {
            Customer = CustomerDetailViewModel.FromEntity(customer),
            Orders = customer.Orders.Select(OrderDetailViewModel.FromEntity)
        };
    }
}