using DataAccess;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Service.Interfaces;
using Service.Models.Requests;
using Service.Models.Responses;

namespace Service;

public class PaperService(AppDbContext context) : IPaperService
{
    public async Task<PaperPropertyDetailViewModel> CreateProperty(PaperPropertyCreateModel property)
    {
        var toProperty = property.ToProperty();
        await context.Properties.AddAsync(toProperty);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new DbUpdateException("An error occurred while trying to insert Order into database.", ex);
        }
        
        return PaperPropertyDetailViewModel.FromEntity(toProperty);
    }
}