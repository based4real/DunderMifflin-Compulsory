using System.ComponentModel.DataAnnotations;
using DataAccess.Models;

namespace Service.Models.Responses;

public class PaperPropertySummaryViewModel
{
    [Required]
    public int Id { get; set; }
    public string? Name { get; set; }

    public static PaperPropertySummaryViewModel FromEntity(Property property)
    {
        return new PaperPropertySummaryViewModel
        {
            Id = property.Id,
            Name = property.PropertyName
        };
    }
}