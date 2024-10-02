using DataAccess;
using DataAccess.Models;
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

    public async Task<List<PaperDetailViewModel>> CreatePapers(List<PaperCreateModel> papers)
    {
        ValidateNoDuplicateNames(papers);
        await ValidateNoExistingNamesInDb(papers);

        var newPapers = papers.Select(p => p.ToPaper()).ToList();
        await AssignPropertiesToPapers(papers, newPapers);
        
        await context.Papers.AddRangeAsync(newPapers);
        
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new DbUpdateException("An error occurred while trying to insert the new papers into the database.", ex);
        }
        
        return newPapers.Select(PaperDetailViewModel.FromEntity).ToList();
    }

    private static void ValidateNoDuplicateNames(List<PaperCreateModel> papers)
    {
        var duplicateNames = papers
            .GroupBy(p => p.Name.ToLower())
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        if (duplicateNames.Count == 0) return;
        
        string errorMessage = duplicateNames.Count == 1
            ? $"The provided paper name '{duplicateNames[0]}' is duplicated. Please provide each paper name only once."
            : $"The following paper names are duplicated: {string.Join(", ", duplicateNames)}. Please provide each paper name only once.";
        throw new BadRequestException(errorMessage);
    }

    private async Task ValidateNoExistingNamesInDb(List<PaperCreateModel> papers)
    {
        var paperNames = papers.Select(p => p.Name.ToLower()).ToList();
        var existingNames = await context.Papers
            .Where(p => paperNames.Contains(p.Name.ToLower()))
            .Select(p => p.Name.ToLower())
            .ToListAsync();

        if (existingNames.Count != 0)
        {
            string errorMessage = existingNames.Count == 1
                ? $"A paper product with the name '{existingNames[0]}' already exists."
                : $"The following paper names already exist: {string.Join(", ", existingNames)}.";
            throw new ConflictException(errorMessage);
        }
    }

    private async Task AssignPropertiesToPapers(List<PaperCreateModel> papers, List<Paper> newPapers)
    {
        var allPropertyIds = papers
            .SelectMany(p => p.PropertyIds ?? Enumerable.Empty<int>())
            .Distinct()
            .ToList();

        var properties = await context.Properties
            .Where(property => allPropertyIds.Contains(property.Id))
            .ToListAsync();

        foreach (var paperModel in papers)
        {
            var newPaper = newPapers.First(p => p.Name == paperModel.Name);
            
            if (paperModel.PropertyIds == null || paperModel.PropertyIds.Count == 0) continue;
                
            var propertyIds = paperModel.PropertyIds.Distinct().ToList();
            newPaper.Properties = properties.Where(property => propertyIds.Contains(property.Id)).ToList();
        }
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

    public async Task Discontinue(List<int> ids)
    {
        var uniqueIds = ids.Distinct().ToList(); // Fjern alle duplikat id
        var validIds = uniqueIds.Where(id => id > 0).ToList(); // Fjern alle ID'er der er 0 eller negativ
        
        ValidateIds(validIds, uniqueIds);
        
        var papers = await context.Papers.Where(paper => validIds.Contains(paper.Id)).ToListAsync();
        var foundIds = ValidateFoundPapers(papers, validIds);

        papers.ForEach(paper => paper.Discontinued = true);
        
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            string idsMessage = foundIds.Count == 1 
                ? $"the paper product with ID {foundIds[0]}" 
                : $"the paper products with IDs {string.Join(", ", foundIds)}";

            throw new DbUpdateException($"An error occurred while trying to discontinue {idsMessage}.", ex);
        }
    }
    
    private static void ValidateIds(List<int> validIds, List<int> uniqueIds)
    {
        if (validIds.Count != 0) return;
        
        string errorMessage = uniqueIds.Count == 1
            ? $"The provided ID {uniqueIds[0]} is invalid. All IDs must be positive numbers greater than 0."
            : $"All provided IDs are invalid. The following IDs are not valid: {string.Join(", ", uniqueIds)}. All IDs must be positive numbers greater than 0.";

        throw new BadRequestException(errorMessage);
    }

    private static List<int> ValidateFoundPapers(List<Paper> papers, List<int> validIds)
    {
        var foundIds = papers.Select(p => p.Id).ToList();
        var invalidIds = validIds.Except(foundIds).ToList();

        if (foundIds.Count != 0) return foundIds;
        
        string errorMessage = invalidIds.Count == 1
            ? $"The provided ID {invalidIds[0]} is invalid."
            : $"No valid paper IDs were provided. The following IDs are invalid: {string.Join(", ", invalidIds)}";

        throw new NotFoundException(errorMessage);
    }

    public async Task Restock(List<PaperRestockUpdateModel> restockModels)
    {
        ValidateNoDuplicateIds(restockModels);

        var paperIds = restockModels.Select(request => request.PaperId).ToList();
        var papers = await context.Papers.Where(paper => paperIds.Contains(paper.Id)).ToListAsync();

        var foundIds = papers.Select(p => p.Id).ToList();
        var invalidIds = paperIds.Except(foundIds).ToList();
        var papersToRestock = papers.Where(p => !p.Discontinued).ToList();
        var discontinuedIds = foundIds.Except(papersToRestock.Select(p => p.Id)).ToList();

        ValidateRestockIds(invalidIds, discontinuedIds, foundIds);
        
        ApplyRestock(papersToRestock, restockModels);
        
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            string idsMessage = foundIds.Count == 1 
                ? $"the paper product with ID {foundIds[0]}" 
                : $"the paper products with IDs {string.Join(", ", foundIds)}";

            throw new DbUpdateException($"An error occurred while trying to restock {idsMessage}.", ex);
        }
    }
    
    private static void ValidateNoDuplicateIds(List<PaperRestockUpdateModel> restockModels)
    {
        var duplicateIds = restockModels
            .GroupBy(request => request.PaperId)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        if (duplicateIds.Count == 0) return;
        
        string errorMessage = duplicateIds.Count == 1
            ? $"The provided ID {duplicateIds[0]} is duplicated. Please provide each paper ID only once."
            : $"The following IDs are duplicated: {string.Join(", ", duplicateIds)}. Please provide each paper ID only once.";

        throw new BadRequestException(errorMessage);
    }
    
    private static void ValidateRestockIds(List<int> invalidIds, List<int> discontinuedIds, List<int> foundIds)
    {
        if (foundIds.Count > 0 && foundIds.Any(id => !discontinuedIds.Contains(id)))
            return;
        
        var messages = new List<string>();

        if (invalidIds.Count != 0)
        {
            string invalidMessage = invalidIds.Count == 1
                ? $"The provided ID {invalidIds[0]} is invalid."
                : $"The following IDs are invalid: {string.Join(", ", invalidIds)}.";
            messages.Add(invalidMessage);
        }

        if (discontinuedIds.Count != 0)
        {
            string discontinuedMessage = discontinuedIds.Count == 1
                ? $"The provided ID {discontinuedIds[0]} corresponds to a discontinued paper and cannot be restocked."
                : $"The following IDs correspond to discontinued papers and cannot be restocked: {string.Join(", ", discontinuedIds)}.";
            messages.Add(discontinuedMessage);
        }

        throw new NotFoundException(string.Join(" ", messages));
    }
    
    private static void ApplyRestock(List<Paper> papersToRestock, List<PaperRestockUpdateModel> restockModels)
    {
        foreach (var paper in papersToRestock)
        {
            var restockModel = restockModels.First(r => r.PaperId == paper.Id);

            // Sørg for at stock ikke går over int.MaxValue (undgå overflow)
            if (paper.Stock > int.MaxValue - restockModel.Amount) paper.Stock = int.MaxValue;
            else                                                  paper.Stock += restockModel.Amount;
        }
    }
}