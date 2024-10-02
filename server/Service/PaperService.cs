using DataAccess;
using Microsoft.EntityFrameworkCore;
using Service.Exceptions;
using Service.Interfaces;
using Service.Models;
using Service.Models.Requests;
using Service.Models.Responses;
using SharedDependencies.Enums;

namespace Service;

public class PaperService(AppDbContext context, IPaperRepository repository) : IPaperService
{
    public async Task<List<PaperDetailViewModel>> All(bool? discontinued)
    {
        var result = await repository.GetFilteredPapers(discontinued).Select(p => PaperDetailViewModel.FromEntity(p)).ToListAsync();
        if (!result.Any())
            throw new NotFoundException("No papers found");

        return result;
    }

    
    public async Task<PaperPagedViewModel> AllPaged(int pageNumber, int itemsPerPage, string? search,
                                                    bool? discontinued, PaperOrderBy orderBy, SortOrder sortOrder,
                                                    List<int>? propertyIds, FilterType filterType)
    {
        var query = repository.GetFilteredPapers(discontinued);

        // Søge efter navn
        if (!string.IsNullOrEmpty(search))
            query = query.Where(paper => paper.Name.ToLower().Contains(search.ToLower()));
        
        // Filter på properties
        if (propertyIds != null && propertyIds.Count != 0)
        {
            if (filterType == FilterType.And) query = query.Where(paper => propertyIds.All(propertyId => paper.Properties.Any(property => property.Id == propertyId)));
            else                              query = query.Where(paper => paper.Properties.Any(property => propertyIds.Contains(property.Id)));
        }
        
        // orderBy og groupBy
        var sortedPapers = repository.OrderBy(query, orderBy, sortOrder);
        
        // Pagination
        var totalItems = await sortedPapers.CountAsync();

        var pagedPapers = await sortedPapers
            .Skip((pageNumber - 1) * itemsPerPage)
            .Take(itemsPerPage)
            .Select(paper => PaperDetailViewModel.FromEntity(paper))
            .ToListAsync();

        return new PaperPagedViewModel
        {
            Papers = pagedPapers,
            PagingInfo = new PagingInfo
            {
                TotalItems = totalItems,
                ItemsPerPage = itemsPerPage,
                CurrentPage = pageNumber
            }
        };
    }

    public async Task<PaperDetailViewModel> CreatePaper(PaperCreateModel paper)
    {
        var newPaper = paper.ToPaper();
        
        var exists = await context.Papers.AnyAsync(p => p.Name.ToLower() == newPaper.Name.ToLower());
        if (exists)
            throw new InvalidOperationException($"A paper product with the name '{newPaper.Name}' already exists.");
        
        
        if (paper.PropertyIds != null && paper.PropertyIds.Count != 0)
        {
            var uniquePropertyIds = paper.PropertyIds.Distinct().ToList();
            
            var properties = await context.Properties.Where(property => uniquePropertyIds.Contains(property.Id)).ToListAsync();
            
            newPaper.Properties = properties;
        }
        
        await context.Papers.AddAsync(newPaper);
        
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new DbUpdateException("An error occurred while trying to insert the new paper into the database.", ex);
        }
        
        return PaperDetailViewModel.FromEntity(newPaper);
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

    public async Task Discontinue(int id)
    {
        var paper = await context.Papers.FindAsync(id);
        if (paper == null)
        {
            throw new NotFoundException($"Paper with ID {id} not found.");
        }
        paper.Discontinued = true;
        
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new DbUpdateException($"An error occurred while trying to discontinue paper with ID {id}.", ex);
        }
    }

    public async Task Restock(int id, int amount)
    {
        var paper = await context.Papers.FindAsync(id);
        if (paper == null)
            throw new NotFoundException($"Paper with ID {id} not found.");
        
        // Sørg for at stock ikke går over int.MaxValue (undgå overflow)
        if (paper.Stock > int.MaxValue - amount) paper.Stock = int.MaxValue;
        else                                     paper.Stock += amount;
        
        
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new DbUpdateException($"An error occurred while trying to restock paper with ID {id}.", ex);
        }
    }
}