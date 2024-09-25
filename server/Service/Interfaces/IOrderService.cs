using Service.Models.Requests;
using Service.Models.Responses;

namespace Service.Interfaces;

public interface IOrderService
{
    public Task<OrderDetailViewModel> Create(OrderCreateModel order);
}