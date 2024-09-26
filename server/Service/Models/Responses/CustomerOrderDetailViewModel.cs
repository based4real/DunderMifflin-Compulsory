using System.Text.Json;
using System.Text.Json.Serialization;
using DataAccess.Models;

namespace Service.Models.Responses;

public class CustomerOrderDetailViewModel : CustomerDetailViewModel
{
    // For at få ordre til at ligge sig under customer i json dataen
    [JsonPropertyOrder(2)]
    public required IEnumerable<OrderDetailViewModel> Orders { get; set; }
    
    // STOP med den dumme warning >:(
    public static new CustomerOrderDetailViewModel FromEntity(Customer customer)
    {
        return new CustomerOrderDetailViewModel
        {
            Id = customer.Id,
            Name = customer.Name,
            Address = customer.Address,
            Phone = customer.Phone,
            Email = customer.Email,
            Orders = customer.Orders.Select(OrderDetailViewModel.FromEntity)
        };
    }
}