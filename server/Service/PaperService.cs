using DataAccess;
using Microsoft.EntityFrameworkCore;
using Service.Enums;
using Service.Exceptions;
using Service.Interfaces;
using Service.Models;
using Service.Models.Requests;
using Service.Models.Responses;

namespace Service;

public class PaperService(AppDbContext context, IPaperRepository repository) : IPaperService
{
    
    private PaperOrderBy ParseOrderBy(string orderBy)
    {
        switch (orderBy.ToLower())
        {
            case "id":
                return PaperOrderBy.Id;
            case "name":
                return PaperOrderBy.Name;
            case "price":
                return PaperOrderBy.Price;
            case "stock":
                return PaperOrderBy.Stock;
            default:
                throw new ArgumentException($"Invalid order by value: {orderBy}");
        }
    }
    
    public async Task<List<PaperDetailViewModel>> All(bool? discontinued)
    {
        var result = await repository.GetFilteredPapers(discontinued).Select(p => PaperDetailViewModel.FromEntity(p)).ToListAsync();
        if (!result.Any())
            throw new NotFoundException("No papers found");

        return result;
    }
    
    public async Task<PaperPagedViewModel> AllPaged(int pageNumber, int itemsPerPage, bool? discontinued, string? orderBy, String sortOrder)
    {
        var query = repository.GetFilteredPapers(discontinued);
        var orderType = PaperOrderBy.Id;
        if (orderBy != null)
            orderType = ParseOrderBy(orderBy);
        
        var sortedPapers = repository.OrderBy(query, orderType, sortOrder);
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