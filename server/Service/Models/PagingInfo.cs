using System.ComponentModel.DataAnnotations;

namespace Service.Models;

public class PagingInfo
{
   [Required]
   public int TotalItems { get; set; }
   
   [Required]
   public int ItemsPerPage { get; set; }
   
   [Required]
   public int CurrentPage { get; set; }
   
   [Required]
   public int TotalPages => TotalPagesCalculation;
   
   private int TotalPagesCalculation => (int) Math.Ceiling((decimal) TotalItems / ItemsPerPage);
}