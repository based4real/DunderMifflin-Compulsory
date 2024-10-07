using System.ComponentModel.DataAnnotations;

namespace Service.Models.Responses;

public class PaperPagedViewModel
{
    public List<PaperDetailViewModel> Papers { get; set; } = null!;
    
    [Required]
    public PagingInfo PagingInfo { get; set; } = new();
}