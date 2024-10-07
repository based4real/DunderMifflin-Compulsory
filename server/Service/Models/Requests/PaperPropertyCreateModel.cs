using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using DataAccess.Models;
using Service.Models.Responses;

namespace Service.Models.Requests;

public class PaperPropertyCreateModel
{
    [Required]
    [MinLength(2)]
    [MaxLength(255)]
    public string Name { get; set; } = null!;
    
    public List<int>? PapersId { get; set; }

    public Property ToProperty()
    {
        return new Property
        {
            PropertyName = Name
        };
    }
}