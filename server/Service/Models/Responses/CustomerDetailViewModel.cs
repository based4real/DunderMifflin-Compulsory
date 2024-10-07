using System.ComponentModel.DataAnnotations;
using DataAccess.Models;

namespace Service.Models.Responses;

public class CustomerDetailViewModel
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = null!;
    
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    
    public static CustomerDetailViewModel FromEntity(Customer customer)
    {
        return new CustomerDetailViewModel
        {
            Id = customer.Id,
            Name = customer.Name,
            Address = customer.Address,
            Phone = customer.Phone,
            Email = customer.Email
        };
    }
}