using DataAccess.Models;

namespace Service.Models.Responses;

public class PaperPropertyDetailViewModel
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public static PaperPropertyDetailViewModel FromEntity(Property property)
    {
        return new PaperPropertyDetailViewModel
        {
            Id = property.Id,
            Name = property.PropertyName
        };
    }
}