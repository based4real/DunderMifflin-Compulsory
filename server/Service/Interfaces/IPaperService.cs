using Service.Models.Requests;
using Service.Models.Responses;
using SharedDependencies.Enums;

namespace Service.Interfaces;

public interface IPaperService
{
    public Task<List<PaperDetailViewModel>> All(bool? discontinued);
    
    public Task<PaperPagedViewModel> AllPaged(int page = 1, int pageSize = 10, string? search = null, bool? discontinued = null,
                                              PaperOrderBy orderBy = PaperOrderBy.Id, SortOrder sortOrder = SortOrder.Asc,
                                              List<int>? propertyIds = null, FilterType filterType = FilterType.Or,
                                              double? minPrice = null, double? maxPrice = null);
    public Task<List<PaperDetailViewModel>> CreatePapers(List<PaperCreateModel> papers);
    public Task<PaperPropertyDetailViewModel> CreateProperty(PaperPropertyCreateModel property);
    public Task Discontinue(List<int> ids);
    public Task Restock(List<PaperRestockUpdateModel> restockModels);
    public Task<List<PaperPropertySummaryViewModel>> AllProperties();
}