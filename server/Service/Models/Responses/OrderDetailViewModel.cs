using System.ComponentModel.DataAnnotations;
using DataAccess.Models;

namespace Service.Models.Responses;

public class OrderDetailViewModel
{
    [Required]
    public int Id { get; set; }
    public string? Status { get; set; } // enum?
    public DateTime OrderDate { get; set; }
    public DateOnly? DeliveryDate { get; set; }
    
    [Required]
    public double TotalPrice { get; set; }
    
    
    [Required]
    public required IEnumerable<OrderEntryDetailViewModel> Entry { get; set; }

    public static OrderDetailViewModel FromEntity(Order order)
    {
        return new OrderDetailViewModel
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            DeliveryDate = order.DeliveryDate,
            Status = order.Status,
            TotalPrice = order.TotalAmount,
            Entry = order.OrderEntries.Select(OrderEntryDetailViewModel.FromEntity)
        };
    }
}