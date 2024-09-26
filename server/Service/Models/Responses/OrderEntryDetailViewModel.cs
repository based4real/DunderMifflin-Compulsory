using DataAccess.Models;

namespace Service.Models.Responses;

public class OrderEntryDetailViewModel
{
    public int Id { get; set; }
    public int? ProductId { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }
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