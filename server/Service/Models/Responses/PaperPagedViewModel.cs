namespace Service.Models.Responses;

public class PaperPagedViewModel
{
    public List<PaperDetailViewModel> Papers { get; set; } = null!;
    public PagingInfo PagingInfo { get; set; } = new();
}