using Microsoft.AspNetCore.Mvc;
using Selflink_api.Services;
using Stripe;
using Stripe.Checkout;
using Selflink_api.Dto;

namespace Selflink_api.Controllers;


[ApiController]
[Route("api/stripe/webhook")]
public class StripeWebHookController : ControllerBase
{
    private readonly ILogger<StripeWebHookController> _logger;

    private readonly string _webhookSecret = Environment.GetEnvironmentVariable("STRIPE_WHSC")!;

    private readonly IOrderService _orderService;

    public StripeWebHookController(ILogger<StripeWebHookController> logger, IOrderService orderService)
    {
        _logger = logger;
        _orderService = orderService;
    }

    [HttpPost(Name = "StripeWebhook")]
    public async Task<IActionResult> Handle()
    {   

        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _webhookSecret,
                throwOnApiVersionMismatch: false // TODO in production, create webhook for 2023+ version of API
            );

            // if (stripeEvent.Type == Events.PaymentIntentSucceeded)
            // {
            //     _logger.LogInformation(" > PaymentIntentSucceeded triggered");
            //     var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            // }

            if ( stripeEvent.Type == Events.CheckoutSessionCompleted)
            {
                _logger.LogInformation(" > Webhook - CheckoutSessionCompleted triggered");
                var checkoutSessionData = stripeEvent.Data.Object as Session;

                if ( checkoutSessionData != null && checkoutSessionData.PaymentStatus.Equals("paid") ) {

                    _logger.LogInformation(" >>  CheckoutSessionCompleted paid");
                    var service = new Stripe.Checkout.SessionService();
                    StripeList<LineItem> checkoutSessionLineItemsList = await service.ListLineItemsAsync(checkoutSessionData.Id);


                    Stripe.ProductService productService = new Stripe.ProductService();
                    Stripe.Product product = await productService.GetAsync(checkoutSessionLineItemsList.Data[0].Price.ProductId);
            
                    // Ref :  reference : https://github.com/sylvainSUPINTERNET/zerecruteur-service/blob/master/src/controllers/webhook.controller.ts
                    // TODO EF + MongoDB 2023 => No support for transaction waiting for 2024 ...
                    await _orderService.SaveOrderAsync(new OrderCreateDto {
                        StripeProductId = checkoutSessionLineItemsList.Data[0].Price.ProductId,
                        StripePriceId = checkoutSessionLineItemsList.Data[0].Price.Id,
                        StripePaymentIntentId = checkoutSessionData.PaymentIntentId,
                        Phone = checkoutSessionData.CustomerDetails.Phone,
                        Email = checkoutSessionData.CustomerDetails.Email,
                        ShippingLine1 = checkoutSessionData.ShippingDetails.Address.Line1,
                        ShippingLine2 = checkoutSessionData.ShippingDetails.Address.Line2,
                        ShippingCity = checkoutSessionData.ShippingDetails.Address.City,
                        ShippingCountry = checkoutSessionData.ShippingDetails.Address.Country,
                        ShippingPostalCode = checkoutSessionData.ShippingDetails.Address.PostalCode,
                        ShippingState = checkoutSessionData.ShippingDetails.Address.State != "" ? checkoutSessionData.ShippingDetails.Address.State : null,
                        QuantityToSend = $"{checkoutSessionLineItemsList.Data[0].Quantity}",
                        Amount = $"{checkoutSessionLineItemsList.Data[0].AmountTotal}", // Include TAX + shipping + quantity total ( express as cents format so 2000 = 20)
                        Status = OrderStatusEnum.PENDING.ToString().ToLower(),
                        ProductName = product.Name,
                        Currency = checkoutSessionData.Currency
                    });

                    _logger.LogInformation("New order Saved with success");

                    // We don't check quantity to disable the link, is up to the user to check it by himself and refund if he can't handle the load !
                
                }
                
            }

            return Ok();
        }
        catch (StripeException e)
        {
            _logger.LogError(e, "StripeWebhook error");

            return BadRequest();
        }
    }
}
