using System.ComponentModel.DataAnnotations;
using DataAccess.Models;

namespace Service.Models.Responses;

public class PaperPropertySummaryViewModel
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = null!;

    public static PaperPropertySummaryViewModel FromEntity(Property property)
    {
        return new PaperPropertySummaryViewModel
        {
            Id = property.Id,
            Name = property.PropertyName
        };
    }
}