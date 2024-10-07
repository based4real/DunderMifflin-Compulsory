using System.ComponentModel.DataAnnotations;

namespace Service.Models.Responses;

public class PaperPagedViewModel
{
    [Required]
    public List<PaperDetailViewModel> Papers { get; set; } = null!;
    
    [Required]
    public PagingInfo PagingInfo { get; set; } = new();
}