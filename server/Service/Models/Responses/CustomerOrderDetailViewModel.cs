using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DataAccess.Models;

namespace Service.Models.Responses;

public class CustomerOrderDetailViewModel : CustomerDetailViewModel
{
    // For at få ordre til at ligge sig under customer i json dataen
    [JsonPropertyOrder(2)]
    [Required]
    public required IEnumerable<OrderDetailViewModel> Orders { get; set; }
    
    public new static CustomerOrderDetailViewModel FromEntity(Customer customer)
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