using DataAccess.Models;
using Service.Models.Requests;
using Service.Models.Responses;
using SharedDependencies.Enums;

namespace Service.Interfaces;

public interface IOrderService
{
    public Task<OrderDetailViewModel> Create(OrderCreateModel order);
    public Task UpdateOrderStatus(int id, OrderStatus orderStatus);
}