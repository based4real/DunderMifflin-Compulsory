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
        // Tjek for dupliat navne
        ValidationHelper.ValidateNoDuplicates(papers, p => p.Name.ToLower(), "paper name");

        // Tjek om navne eksister i db
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
        
        var newPapers = papers.Select(p => p.ToPaper()).ToList();

        // Tildel properties til papir
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

            if (paperModel.PropertyIds == null || !paperModel.PropertyIds.Any()) continue;

            var propertyIds = paperModel.PropertyIds.Distinct().ToList();
            newPaper.Properties = properties.Where(property => propertyIds.Contains(property.Id)).ToList();
        }
        
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
        var uniqueIds = ids.Distinct().ToList();
        
        var validIds = ValidationHelper.FilterValidIds(uniqueIds, "paper", out var invalidIds);
    
        if (validIds.Count == 0)
            throw new BadRequestException("All provided IDs are invalid. Please provide valid paper IDs greater than 0.");
        
        var papers = await context.Papers.Where(paper => validIds.Contains(paper.Id)).ToListAsync();
        var validItemsFound = ValidationHelper.ValidateItemsExistence(validIds, papers, p => p.Id, out var notFoundIds);
        
        if (!validItemsFound)
            throw new NotFoundException("No valid paper IDs were provided for discontinuation.");
        
        var papersToDiscontinue = papers.Where(p => !invalidIds.Contains(p.Id) && !notFoundIds.Contains(p.Id)).ToList();

        papersToDiscontinue.ForEach(paper => paper.Discontinued = true);
    
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            var idsMessage = uniqueIds.Count == 1 
                ? $"the paper product with ID {uniqueIds[0]}" 
                : $"the paper products with IDs {string.Join(", ", uniqueIds)}";

            throw new DbUpdateException($"An error occurred while trying to discontinue {idsMessage}.", ex);
        }
    }
    
    public async Task Restock(List<PaperRestockUpdateModel> restockModels)
    {
        ValidationHelper.ValidateNoDuplicates(restockModels, r => r.PaperId, "paper ID");

        var paperIds = restockModels.Select(request => request.PaperId).ToList();
        var papers = await context.Papers.Where(paper => paperIds.Contains(paper.Id)).ToListAsync();

        var validItemsFound = ValidationHelper.ValidateItemsExistence(paperIds, papers, p => p.Id, out var invalidIds);

        if (!validItemsFound)
            throw new NotFoundException("No valid paper IDs were provided for restocking.");
        
        var papersToRestock = papers.Where(p => !p.Discontinued && !invalidIds.Contains(p.Id)).ToList();

        if (papersToRestock.Count == 0)
            throw new NotFoundException("No valid, non-discontinued paper IDs were provided for restocking.");

        foreach (var paper in papersToRestock)
        {
            var restockModel = restockModels.First(r => r.PaperId == paper.Id);

            // Sørg for at stock ikke går over int.MaxValue (undgå overflow)
            if (paper.Stock > int.MaxValue - restockModel.Amount) paper.Stock = int.MaxValue;
            else                                                  paper.Stock += restockModel.Amount;
        }

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            var idsMessage = paperIds.Count == 1 
                ? $"the paper product with ID {paperIds[0]}" 
                : $"the paper products with IDs {string.Join(", ", paperIds)}";

            throw new DbUpdateException($"An error occurred while trying to restock {idsMessage}.", ex);
        }
    }
    
    public async Task<List<PaperPropertySummaryViewModel>> AllProperties()
    {
        return await context.Properties
                            .OrderBy(property => property.PropertyName.ToLower())
                            .Select(property => PaperPropertySummaryViewModel.FromEntity(property))
                            .ToListAsync();
    }
}