using Selflink_api.Dto;
using Selflink_api.Db;
using Stripe;
using Selflink_api.Db.Models;

namespace Selflink_api.Services;

public class LinkService : ILinkService
{
    private readonly ILogger<LinkService> _logger;

    private readonly SelflinkDbContext _db;


    public LinkService(ILogger<LinkService> logger, SelflinkDbContext db) {
        
        _logger = logger;
        _db = db;
    }

    public async Task<LinksDto?> SaveLink(LinksCreateDto linksCreateDto)
    {
        _logger.LogInformation($"SaveLink : {linksCreateDto.Name}");
        // TODO => transaction supported with mongodb + EF in 2024 ...
        // var transactionSave = _db.Database.BeginTransaction();

        // TODO => must be from claims token
        string sub = "123";


        // TODO : price  ??? =>   reqObj.req.body.price = reqObj.req.body.price * 100;
        long priceAsDouble = Convert.ToInt64(Convert.ToDecimal(linksCreateDto.PriceUnit) * 100);


        // https://learn.microsoft.com/en-us/ef/core/saving/transactions
        try {

            var StripeOptionsProductCreate = new Stripe.ProductCreateOptions
            {
                Name = linksCreateDto.ProductName,
                Description = linksCreateDto.description,
                Images = linksCreateDto.ProductImage,
                Metadata = new Dictionary<string, string>
                {
                    { "price", $"{priceAsDouble}" },
                    { "currency", linksCreateDto.Currency.ToLower() },
                },
            };
            Stripe.Product product = new Stripe.ProductService().Create(StripeOptionsProductCreate);
        
            var StripeOptionsPriceCreate = new Stripe.PriceCreateOptions
            {
                UnitAmount = priceAsDouble,
                Currency = product.Metadata["currency"],
                Product = product.Id,
            };
            Stripe.Price price = new Stripe.PriceService().Create(StripeOptionsPriceCreate);


            var StripeOptionsPaymentLinkCreate = new Stripe.PaymentLinkCreateOptions
            {
                LineItems = new List<Stripe.PaymentLinkLineItemOptions>
                {
                    new Stripe.PaymentLinkLineItemOptions
                    {
                        Price = price.Id,
                        Quantity = Convert.ToInt64(linksCreateDto.QuantityStock),
                        AdjustableQuantity = new Stripe.PaymentLinkLineItemAdjustableQuantityOptions
                        {
                            Enabled = true,
                            Maximum = Convert.ToInt64(linksCreateDto.QuantityStock),
                            Minimum = 1,
                        }

                    },
                },

                ShippingAddressCollection = new Stripe.PaymentLinkShippingAddressCollectionOptions
                {
                    AllowedCountries = linksCreateDto.ShippingCountries,
                },
                PhoneNumberCollection =  new Stripe.PaymentLinkPhoneNumberCollectionOptions
                {
                    Enabled = true,
                },
                AllowPromotionCodes = true,
                AutomaticTax =  new Stripe.PaymentLinkAutomaticTaxOptions {
                    Enabled = true,
                },
                AfterCompletion = new Stripe.PaymentLinkAfterCompletionOptions {
                    Type = "hosted_confirmation",
                    HostedConfirmation = new Stripe.PaymentLinkAfterCompletionHostedConfirmationOptions {
                        CustomMessage = "Thanks for your order!"
                    }
                }
            };

            PaymentLink paymentLink = new Stripe.PaymentLinkService().Create(StripeOptionsPaymentLinkCreate);

            await _db.Links.AddAsync(new Link {
                Name = linksCreateDto.Name,
                Sub = sub,
                Iban = linksCreateDto.Iban,
                PaymentUrl = paymentLink.Url,
                StripeId = product.Id
            });

            await _db.SaveChangesAsync();

        // TODO => transaction supported with mongodb + EF in 2024 ...
            // await transactionSave.CommitAsync();

            return new LinksDto {
                Id = product.Id,
                Name = linksCreateDto.Name,
                Sub = sub,
                Iban = linksCreateDto.Iban,
                PaymentUrl = paymentLink.Url,
                StripeId = product.Id
            };
            
        } catch ( Exception e ) {

            _logger.LogError($"Error during save link for {linksCreateDto.Name} : {e.Message}");
            // TODO => transaction supported with mongodb + EF in 2024 ...
            // await transactionSave.RollbackAsync();
            
            return null;

        }

    }


}