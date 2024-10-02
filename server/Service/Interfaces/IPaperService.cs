using DataAccess.Models;
using Service.Enums;
using Service.Models.Requests;
using Service.Models.Responses;

namespace Service.Interfaces;

public interface IPaperService
{
    public Task<List<PaperDetailViewModel>> All(bool? discontinued);
    
    public Task<PaperPagedViewModel> AllPaged(int page = 1, int pageSize = 10, bool? discontinued = null, string? orderBy = null, string? sortOrder = "asc");
    public Task<PaperPropertyDetailViewModel> CreateProperty(PaperPropertyCreateModel property);
}