using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace Selflink_api.Controllers;

[ApiController]
[Route("api/stripe/webhook")]
public class StripeWebHookController : ControllerBase
{
    private readonly ILogger<StripeWebHookController> _logger;

    private readonly string _webhookSecret = Environment.GetEnvironmentVariable("STRIPE_WHSC")!;

    public StripeWebHookController(ILogger<StripeWebHookController> logger)
    {
        _logger = logger;
    }

    [HttpPost(Name = "StripeWebhook")]
    public async Task<IActionResult> Handle()
    {   

        _logger.LogInformation("StripeWebhook triggered");

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
                _logger.LogInformation(" > CheckoutSessionCompleted triggered");
                var checkoutSessionData = stripeEvent.Data.Object as Session;

                if ( checkoutSessionData != null && checkoutSessionData.PaymentStatus.Equals("paid") ) {
                    _logger.LogInformation(" >>  CheckoutSessionCompleted paid");
                    var service = new Stripe.Checkout.SessionService();
                    StripeList<LineItem> checkoutSessionLineItemsList = await service.ListLineItemsAsync(checkoutSessionData.Id);
                    

                    // TODO : reference : https://github.com/sylvainSUPINTERNET/zerecruteur-service/blob/master/src/controllers/webhook.controller.ts
                    // Create order entity ( each item hold reference to product / price / user detail )
                    // Check quantity, if too low => disable the link
                    // Create index on element of search / finish the order page list + pagination with it ( try to don't use naive limit / offset ) 

                    
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
