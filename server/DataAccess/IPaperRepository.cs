using DataAccess.Models;

namespace DataAccess;

public interface IPaperRepository
{
    public IQueryable<Paper> GetFilteredPapers(bool? discontinued);
}