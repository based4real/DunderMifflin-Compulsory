using System.Text.Json.Serialization;

namespace Service.Models;

public class PagingInfo
{
   public int TotalItems { get; set; }
   public int ItemsPerPage { get; set; }
   public int CurrentPage { get; set; }

   [JsonPropertyName("totalPages")] // Gør sådan at totalPages kommer med i JSON respons
   public int TotalPages => TotalPagesCalculation;
   
   private int TotalPagesCalculation => (int) Math.Ceiling((decimal) TotalItems / ItemsPerPage);
}