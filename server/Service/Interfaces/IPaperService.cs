using DataAccess.Models;
using Service.Models.Requests;
using Service.Models.Responses;

namespace Service.Interfaces;

public interface IPaperService
{
    public Task<PaperPropertyDetailViewModel> CreateProperty(PaperPropertyCreateModel property);
}