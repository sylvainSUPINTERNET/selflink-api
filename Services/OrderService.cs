using Selflink_api.Db;
using Selflink_api.Dto;

namespace Selflink_api.Services;


public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;

    private readonly SelflinkDbContext _db;
    

    public OrderService(ILogger<OrderService> logger, SelflinkDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task<OrderCreateDto?> SaveOrderAsync(OrderCreateDto orderCreateDto)
    {
        _logger.LogInformation("SaveOrder triggered");

        if ( orderCreateDto == null ) {
            throw new Exception("orderCreateDto is null, can't save it");
        }

        await _db.Orders.AddAsync(new Db.Models.Order
        {
            StripeProductId = orderCreateDto.StripeProductId,
            StripePriceId = orderCreateDto.StripePriceId,
            StripePaymentIntentId = orderCreateDto.StripePaymentIntentId,
            Phone = orderCreateDto.Phone,
            Email = orderCreateDto.Email,
            ShippingLine1 = orderCreateDto.ShippingLine1,
            ShippingLine2 = orderCreateDto.ShippingLine2,
            ShippingCity = orderCreateDto.ShippingCity,
            ShippingPostalCode = orderCreateDto.ShippingPostalCode,
            ShippingState = orderCreateDto.ShippingState,
            ShippingCountry = orderCreateDto.ShippingCountry,
            QuantityToSend = orderCreateDto.QuantityToSend,
            Amount = orderCreateDto.Amount,
            Currency = orderCreateDto.Currency,
            Status = orderCreateDto.Status
        });

        await _db.SaveChangesAsync();

        return Task.FromResult(orderCreateDto).Result;
            
    }
}