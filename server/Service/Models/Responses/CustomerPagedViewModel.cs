using System.ComponentModel.DataAnnotations;

namespace Service.Models.Responses;

public class CustomerPagedViewModel
{
    [Required]
    public List<CustomerOrderDetailViewModel> Customers { get; set; } = null!;
    
    [Required]
    public PagingInfo PagingInfo { get; set; } = new();
}