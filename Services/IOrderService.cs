using Selflink_api.Dto;

namespace Selflink_api.Services;

public interface IOrderService
{
    public Task<OrderCreateDto?> SaveOrderAsync(OrderCreateDto orderCreateDto);
}