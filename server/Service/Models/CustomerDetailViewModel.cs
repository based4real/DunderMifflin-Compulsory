using DataAccess.Models;

namespace Service.TransferModels;

public class CustomerDetailViewModel
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    
    public required IEnumerable<Item> Orders { get; set; }

    public class Item
    {
        public DateTime OrderDate { get; set; }
        public DateOnly? DeliveryDate { get; set; }
    }

    public static CustomerDetailViewModel FromEntity(Customer customer)
    {
        return new CustomerDetailViewModel
        {
            Id = customer!.Id,
            Name = customer.Name,
            Address = customer.Address,
            Phone = customer.Phone,
            Email = customer.Email,
            Orders = customer.Orders.Select(order => new Item
            {
                OrderDate = order.OrderDate,
                DeliveryDate = order.DeliveryDate
            })
        };
    }
}