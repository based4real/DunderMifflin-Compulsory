using DataAccess.Models;
using Service.Models.Responses;

namespace Service.Interfaces;

public interface IPaperService
{
    public Task<PaperPropertyDetailViewModel> CreateProperty(PaperPropertyDetailViewModel property);
}