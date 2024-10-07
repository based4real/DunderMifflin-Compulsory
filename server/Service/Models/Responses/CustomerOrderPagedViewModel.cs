using System.ComponentModel.DataAnnotations;

namespace Service.Models.Responses;

public class CustomerOrderPagedViewModel
{
    [Required]
    public CustomerOrderDetailViewModel CustomerDetails { get; set; } = null!;
    
    [Required]
    public PagingInfo PagingInfo { get; set; } = new();
}