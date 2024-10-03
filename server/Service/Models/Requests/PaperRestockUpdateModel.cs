using System.ComponentModel.DataAnnotations;

namespace Service.Models.Requests;

public class PaperRestockUpdateModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Paper ID must be a positive number greater than 0.")]
    public int PaperId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Restock amount must be a positive number.")]
    public int Amount { get; set; }
}