using DataAccess.Models;
using Service.Models.Order;

namespace Service.Interfaces;

public interface IOrderService
{
    public Task<OrderDetailViewModel> Create(OrderCreateModel order);
}