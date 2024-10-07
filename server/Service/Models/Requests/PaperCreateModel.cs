using System.ComponentModel.DataAnnotations;
using DataAccess.Models;

namespace Service.Models.Requests;

public class PaperCreateModel
{
    [Required]
    [MinLength(2)]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public double Price { get; set; }
    
    public List<int>? PropertyIds { get; set; }

    public Paper ToPaper()
    {
        return new Paper
        {
            Name = Name,
            Stock = Stock,
            Price = Price,
            Discontinued = false
        };
    }
}