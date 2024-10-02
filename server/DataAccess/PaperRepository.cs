using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Service.Enums;

namespace DataAccess;

public class PaperRepository(AppDbContext context) : IPaperRepository
{
    public IQueryable<Paper> GetFilteredPapers(bool? discontinued)
    {
        var query = context.Papers
            .Include(paper => paper.Properties)
            .AsQueryable();

        if (discontinued.HasValue)
            query = query.Where(paper => paper.Discontinued == discontinued.Value);

        return query;
    }
    
    public IQueryable<Paper> OrderBy(IQueryable<Paper> query, PaperOrderBy orderBy, string sortOrder)
    {
        switch (orderBy)
        {
            case PaperOrderBy.Id:
                return sortOrder == "desc" ? query.OrderByDescending(paper => paper.Id) : query.OrderBy(paper => paper.Id);
            case PaperOrderBy.Name:
                return sortOrder == "desc" ? query.OrderByDescending(paper => paper.Name) : query.OrderBy(paper => paper.Name);
            case PaperOrderBy.Price:
                return sortOrder == "desc" ? query.OrderByDescending(paper => paper.Price) : query.OrderBy(paper => paper.Price);
            case PaperOrderBy.Stock:
                return sortOrder == "desc" ? query.OrderByDescending(paper => paper.Stock) : query.OrderBy(paper => paper.Stock);
            default:
                return query.OrderBy(paper => paper.Id);
        }
    }
}