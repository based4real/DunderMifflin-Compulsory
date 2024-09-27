using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using DataAccess.Models;
using Service.Models.Responses;

namespace Service.Models.Requests;

public class PaperPropertyCreateModel
{
    [Required]
    [NotNull]
    [MinLength(2)]
    public string? name { get; set; }
    
    public List<int>? PapersId { get; set; }

    public Property ToProperty()
    {
        return new Property
        {
            PropertyName = name
        };
    }
}