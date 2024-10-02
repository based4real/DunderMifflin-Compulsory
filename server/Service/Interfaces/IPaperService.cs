using Service.Models.Requests;
using Service.Models.Responses;
using SharedDependencies.Enums;

namespace Service.Interfaces;

public interface IPaperService
{
    public Task<List<PaperDetailViewModel>> All(bool? discontinued);
    
    public Task<PaperPagedViewModel> AllPaged(int page = 1, int pageSize = 10, string? search = null, bool? discontinued = null,
                                              PaperOrderBy orderBy = PaperOrderBy.Id, SortOrder sortOrder = SortOrder.Asc,
                                              List<int>? propertyIds = null, FilterType filterType = FilterType.Or);
    public Task<PaperDetailViewModel> CreatePaper(PaperCreateModel paper);
    public Task<PaperPropertyDetailViewModel> CreateProperty(PaperPropertyCreateModel property);
    public Task Discontinue(int id);
    public Task Restock(int id, int amount);
}