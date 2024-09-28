using System.ComponentModel.DataAnnotations;
using DataAccess.Models;

namespace Service.Models.Responses;

public class PaperDetailViewModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool Discontinued { get; set; }
    public int Stock { get; set; }
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