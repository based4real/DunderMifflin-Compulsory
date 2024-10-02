using DataAccess;
using Microsoft.EntityFrameworkCore;
using Service.Exceptions;
using Service.Interfaces;
using Service.Models.Requests;
using Service.Models.Responses;

namespace Service;

public class PaperService(AppDbContext context) : IPaperService
{
    public async Task<List<PaperDetailViewModel>> All(bool? discontinued)
    {
        // Lav til en IQueryable så vi kan tilføje "filteret"
        var query = context.Papers
            .Include(property => property.Properties)
            .AsQueryable();

        if (discontinued.HasValue)
            query = query.Where(paper => paper.Discontinued == discontinued.Value);
        
        var result = await query
            .Select(paper => PaperDetailViewModel.FromEntity(paper))
            .ToListAsync();

        if (!result.Any())
            throw new NotFoundException("No papers found");

        return result;
    }
    
    public async Task<PaperPropertyDetailViewModel> CreateProperty(PaperPropertyCreateModel property)
    {
        var toProperty = property.ToProperty();

        var exists = await context.Properties
            .AnyAsync(p => p.PropertyName == toProperty.PropertyName);
        
        if (exists)
            throw new InvalidOperationException("The property already exists");

        // Indsæt for det paperId
        if (property.PapersId != null && property.PapersId.Count > 0)
        {
            var papers = await context.Papers.Where(p => property.PapersId.Contains(p.Id)).ToListAsync();
            if (papers.Count != property.PapersId.Count)
                throw new InvalidOperationException("One or more provided paper IDs does not exist.");

            toProperty.Papers = papers;
        }
        
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