using System.ComponentModel.DataAnnotations;

namespace Service.Models.Requests;


public class OrderCreateModel
{
    [Required(ErrorMessage = "CustomerId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "CustomerId must be greater than 0")]
    public int CustomerId { get; set; }
    
    public required IEnumerable<OrderCreateEntryModel> OrderEntries { get; set; }

    public DataAccess.Models.Order ToOrder()
    {
        return new DataAccess.Models.Order
        {
            CustomerId = CustomerId,
            OrderDate = DateTime.UtcNow,
            DeliveryDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
        };
    }
}