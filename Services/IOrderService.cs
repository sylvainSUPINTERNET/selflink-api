using Selflink_api.Db.Models;
using Selflink_api.Dto;
using Selflink_api.Dto.Api;

namespace Selflink_api.Services;

public interface IOrderService
{
    public Task<OrderCreateDto?> SaveOrderAsync(OrderCreateDto orderCreateDto);

    public Task<List<Order>> GetOrdersAsync(string stripeProductId, string idLast, int limit, List<string> status);

    public Task<OrderRefundDto> RefundOrderAsync(OrderRefundDto orderRefundDto);

    public Task<bool> UpdateStatusOrderAsync(List<OrderStatusDto> orderStatusDto);
}