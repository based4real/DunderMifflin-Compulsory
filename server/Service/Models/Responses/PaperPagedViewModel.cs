namespace Service.Models.Responses;

public class PaperPagedViewModel
{
    public PaperDetailViewModel PaperDetailViewModel { get; set; } = null!;
    public PagingInfo PagingInfo { get; set; } = new();
}