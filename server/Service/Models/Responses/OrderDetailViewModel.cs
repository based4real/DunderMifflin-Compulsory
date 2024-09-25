using DataAccess.Models;

namespace Service.Models.Responses;

public class OrderDetailViewModel
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public DateOnly? DeliveryDate { get; set; }
    public string? Status { get; set; } // enum?
    public double Price { get; set; }
    
    public required IEnumerable<Item> Orders { get; set; }

    public class Item
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int? ProductId { get; set; }
    }

    public static OrderDetailViewModel FromEntity(Order order)
    {
        return new OrderDetailViewModel
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            DeliveryDate = order.DeliveryDate,
            Status = order.Status,
            Price = order.TotalAmount,
            Orders = order.OrderEntries.Select(x => new Item
            {
                Id = x.Id,
                Quantity = x.Quantity,
                ProductId = x.ProductId
            })
        };
    }
}