using System.ComponentModel.DataAnnotations;
using DataAccess.Models;

namespace Service.Models.Responses;

public class OrderEntryDetailViewModel
{
    [Required]
    public int Id { get; set; }
    public int? ProductId { get; set; }
    public string? ProductName { get; set; }
    
    [Required]
    public int Quantity { get; set; }
    
    [Required]
    public double Price { get; set; }
    
    [Required]
    public double TotalPrice { get; set; }

    public static OrderEntryDetailViewModel FromEntity(OrderEntry entry)
    {
        return new OrderEntryDetailViewModel
        {
            Id = entry.Id,
            ProductId = entry.ProductId,
            ProductName = entry.Product?.Name ?? "Not available",
            Price = entry.Product?.Price ?? 0,
            Quantity = entry.Quantity,
            TotalPrice = entry.Product?.Price * entry.Quantity ?? 0
        };
    }
}