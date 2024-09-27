using DataAccess;
using DataAccess.Models;
using Service.Interfaces;
using Service.Models.Responses;

namespace Service;

public class PaperService(AppDbContext context) : IPaperService
{
    public async Task<PaperPropertyDetailViewModel> CreateProperty(string name)
    {
        var property = new Property
        {
            PropertyName = name
        };
        
        
    }
}