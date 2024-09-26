using DataAccess.Models;

namespace Service.Models.Responses;

public class OrderDetailViewModel
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public DateOnly? DeliveryDate { get; set; }
    public string? Status { get; set; } // enum?
    public double TotalPrice { get; set; }
    
    public required IEnumerable<Item> Entry { get; set; }

    public class Item
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int? ProductId { get; set; }
        public double Price { get; set; }
        public double TotalPrice { get; set; }
    }

    public static OrderDetailViewModel FromEntity(Order order)
    {
        return new OrderDetailViewModel
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            DeliveryDate = order.DeliveryDate,
            Status = order.Status,
            TotalPrice = order.TotalAmount,
            Entry = order.OrderEntries.Select(x => new Item
            {
                Id = x.Id,
                Quantity = x.Quantity,
                ProductId = x.ProductId,
                Price = x.Product?.Price ?? 0,
                TotalPrice = x.Product?.Price * x.Quantity ?? 0
            })
        };
    }
}