using System.ComponentModel.DataAnnotations;
using DataAccess.Models;
using Newtonsoft.Json;

namespace Service.Models.Responses;

public class CustomerOrderDetailViewModel : CustomerDetailViewModel
{
    [JsonProperty(Order = 2)]
    [Required]
    public int TotalOrders { get; set; }

    // For at få ordre til at ligge sig under customer i json dataen
    [JsonProperty(Order = 3, NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<OrderDetailViewModel>? Orders { get; set; }
    
    public static CustomerOrderDetailViewModel FromEntity(Customer customer, bool includeOrderHistory = false)
    {
        return new CustomerOrderDetailViewModel
        {
            Id = customer.Id,
            Name = customer.Name,
            Address = customer.Address,
            Phone = customer.Phone,
            Email = customer.Email,
            TotalOrders = customer.Orders.Count,
            Orders = includeOrderHistory ? customer.Orders.Select(OrderDetailViewModel.FromEntity) : null
        };
    }
}