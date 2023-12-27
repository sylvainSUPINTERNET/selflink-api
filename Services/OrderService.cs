using MongoDB.Bson;
using MongoDB.Driver;
using Selflink_api.Db;
using Selflink_api.Db.Models;
using Selflink_api.Dto;
using Selflink_api.Dto.Api;

namespace Selflink_api.Services;


public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;

    private readonly IMongoDbClientSingleton _db;

    private readonly IMongoCollection<Order> _orderCollection;

    public OrderService(ILogger<OrderService> logger, IMongoDbClientSingleton db)
    {
        _logger = logger;
        _db = db;
        _orderCollection = db.GetDatabase().GetCollection<Order>(db.GetOrderCollectionName());
    }

    public Task<List<Order>> GetOrdersAsync(string sub)
    {
        throw new NotImplementedException();
    }

    public async Task<OrderCreateDto?> SaveOrderAsync(OrderCreateDto orderCreateDto)
    {
        _logger.LogInformation("SaveOrder triggered");

        if ( orderCreateDto == null ) {
            throw new Exception("orderCreateDto is null, can't save it");
        }

        await _orderCollection.InsertOneAsync(new Db.Models.Order
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

        return Task.FromResult(orderCreateDto).Result;
            
    }

    /**
    * GetOrdersAsync : Get all orders for a product. If idLast is provider, using it as keyset to pagination and get onlyt the next orders after this ID ( objectId timestamp is contained in the id)
    * 
    * @param string stripeProductId
    * @param string idLast
    * @param int limit
    * 
    * @return List<Order>
    */
    public async Task<List<Order>> GetOrdersAsync(string stripeProductId, string idLast, int limit)
    {
        _logger.LogInformation($"GetOrders : {stripeProductId} for {idLast}");

        if (string.IsNullOrEmpty(stripeProductId)) {
            throw new ArgumentException("stripeProductId is empty");
        }

        var filter = Builders<Order>.Filter.Eq(l => l.StripeProductId, stripeProductId);

        if ( !string.IsNullOrEmpty(idLast) ) {
            var lastObjectId = new ObjectId(idLast);
            var idFilter = Builders<Order>.Filter.Gt("_id", lastObjectId);
            filter = Builders<Order>.Filter.And(filter, idFilter);
        }

        return await _orderCollection
            .Find(filter)
            .SortBy(l => l.Id)
            .Limit(limit)
            .ToListAsync();
    }


    public async Task<OrderRefundDto> RefundOrderAsync(OrderRefundDto orderRefundDto) {

        Stripe.PaymentIntentService paymentIntentService = new Stripe.PaymentIntentService();
        Stripe.PaymentIntent paymentIntent = paymentIntentService.Get(orderRefundDto.StripePaymentIntentId);

        if ( paymentIntent == null ) {
            throw new Exception("PaymentIntent not found");
        }

        if ( paymentIntent.LatestCharge == null ) {
            throw new Exception("PaymentIntent.LatestCharge is null");
        }

        Stripe.ChargeService chargeService = new Stripe.ChargeService();
        Stripe.Charge charge = chargeService.Get(paymentIntent.LatestCharge.Id);
        
        if ( charge == null ) {
            throw new Exception("Charge not found");
        }

        if ( charge.Refunded ) {
            throw new Exception("Charge already refunded");
        }

        Stripe.RefundCreateOptions refundCreateOptions = new Stripe.RefundCreateOptions();
        refundCreateOptions.Amount = charge.Amount;
        refundCreateOptions.Charge = charge.Id;

        Stripe.RefundService refundService = new Stripe.RefundService();
        Stripe.Refund refund = refundService.Create(refundCreateOptions);

        await _orderCollection.UpdateOneAsync(
            Builders<Order>.Filter.Eq(o => o.StripePaymentIntentId, orderRefundDto.StripePaymentIntentId),
            Builders<Order>.Update.Set(o => o.Status, "refunded")
        );

        
        return Task.FromResult(orderRefundDto).Result;
    }

    public async Task<bool> UpdateStatusOrderAsync(List<OrderStatusDto> orderStatusListDto) {

        if ( orderStatusListDto ==null || orderStatusListDto.Count == 0 ) {
            throw new Exception("orderStatusDto is empty");
        }

        List<string> orderIdsPending = orderStatusListDto.Where(o=>o.Status == "pending").Select(o =>o.Id).ToList();
        List<string> orderIdsPendingToSend = orderStatusListDto.Where(o=>o.Status == "send").Select(o => o.Id).ToList();

        if ( orderIdsPending.Count > 0 ) {
            await _orderCollection.UpdateManyAsync(
                Builders<Order>.Filter.In(o => o.Id, orderIdsPending),
                Builders<Order>.Update.Set(o => o.Status, "pending")
            );
        }

        if ( orderIdsPendingToSend.Count > 0 ) {
            await _orderCollection.UpdateManyAsync(
                Builders<Order>.Filter.In(o => o.Id, orderIdsPendingToSend),
                Builders<Order>.Update.Set(o => o.Status, "send")
            );
        }
        
        return Task.FromResult(true).Result;
    }
}