using DataAccess.Models;
using Service.Enums;

namespace DataAccess;

public interface IPaperRepository
{
    public IQueryable<Paper> GetFilteredPapers(bool? discontinued);
    
    public IQueryable<Paper> OrderBy(IQueryable<Paper> query, PaperOrderBy orderBy, string sortOrder);
}