using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

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
}