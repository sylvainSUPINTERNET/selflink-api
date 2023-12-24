using Selflink_api.Db.Models;
using Selflink_api.Dto;

namespace Selflink_api.Services;

public interface IOrderService
{
    public Task<OrderCreateDto?> SaveOrderAsync(OrderCreateDto orderCreateDto);

    public Task<List<Order>> GetOrdersAsync(string stripeProductId, string idLast, int limit);
}