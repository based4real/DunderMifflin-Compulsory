using System.ComponentModel.DataAnnotations;
using DataAccess.Models;

namespace Service.Models.Responses;

public class PaperDetailViewModel
{
    [Required]
    public int Id { get; set; }
    
    public string? Name { get; set; }
    
    [Required]
    public bool Discontinued { get; set; }
    
    [Required]
    public int Stock { get; set; }
    
    [Required]
    public double Price { get; set; }
    
    public List<PaperPropertyDetailViewModel>? Properties { get; set; }

    public static PaperDetailViewModel FromEntity(Paper paper)
    {
        return new PaperDetailViewModel
        {
            Id = paper.Id,
            Name = paper.Name,
            Discontinued = paper.Discontinued,
            Stock = paper.Stock,
            Price = paper.Price,
            Properties = paper.Properties.Select(p => new PaperPropertyDetailViewModel
            {
                Id = p.Id,
                Name = p.PropertyName
            }).ToList()
        };
    }
}