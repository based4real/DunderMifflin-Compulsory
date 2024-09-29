namespace Service.Models.Responses;

public class CustomerOrderPagedViewModel
{
    public CustomerOrderDetailViewModel CustomerDetails { get; set; } = null!;
    public PagingInfo PagingInfo { get; set; } = new();
}