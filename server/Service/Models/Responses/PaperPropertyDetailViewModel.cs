using System.ComponentModel.DataAnnotations;
using DataAccess.Models;
using Newtonsoft.Json;

namespace Service.Models.Responses;

public class PaperPropertyDetailViewModel
{
    [Required]
    public int Id { get; set; }
    public string? Name { get; set; }
    
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] // Ignorer denne hvis værdi er null
    public List<PaperDetailViewModel>? PaperPropertyDetails { get; set; }

    public static PaperPropertyDetailViewModel FromEntity(Property property)
    {
        return new PaperPropertyDetailViewModel
        {
            Id = property.Id,
            Name = property.PropertyName,
            PaperPropertyDetails = property.Papers.Select(p => new PaperDetailViewModel
            {
                Id = p.Id,
                Discontinued = p.Discontinued,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock,
                Properties = p.Properties.Select(paperProperty => new PaperPropertyDetailViewModel
                {
                    Id = paperProperty.Id,
                    Name = paperProperty.PropertyName
                }).ToList()
            }).ToList()
        };
    }
}