using DataAccess.Models;
using SharedDependencies.Enums;

namespace DataAccess;

public interface IPaperRepository
{
    public IQueryable<Paper> GetFilteredPapers(bool? discontinued);
    
    public IQueryable<Paper> OrderBy(IQueryable<Paper> query, PaperOrderBy orderBy, SortOrder sortOrder);
}