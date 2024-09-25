using System.ComponentModel.DataAnnotations;
using DataAccess.Models;

namespace Service.Models.Requests;

public class OrderCreateEntryModel
{
    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "ProductId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int ProductId { get; set; }

    public OrderEntry ToOrderEntry()
    {
        return new OrderEntry
        {
            ProductId = ProductId,
            Quantity = Quantity
        };
    }
    
    public OrderEntry ToOrderEntry(Order order)
    {
        return new OrderEntry
        {
            ProductId = ProductId,
            Quantity = Quantity,
            OrderId = order.Id
        };
    }
}