using Selflink_api.Dto;
using Selflink_api.Db;
using Stripe;
using Selflink_api.Db.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.EntityFrameworkCore;


namespace Selflink_api.Services;

public class LinkService : ILinkService
{
    private readonly ILogger<LinkService> _logger;

    private readonly IMongoCollection<Link> _linkCollection;

    private readonly IMongoDbClientSingleton _db;

    public LinkService(ILogger<LinkService> logger, IMongoDbClientSingleton db) {
        
        _logger = logger;
        _db = db;
        _linkCollection = db.GetDatabase().GetCollection<Link>(db.GetLinkCollectionName());
        
    }

    public async Task<LinksDto?> SaveLink(LinksCreateDto linksCreateDto)
    {
        _logger.LogInformation($"SaveLink : {linksCreateDto.Name}");
        
        // TODO => must be from claims token
        string sub = "123";

        using (var session = await _db.GetClient().StartSessionAsync()) {
            session.StartTransaction();

            try {

                var StripeOptionsProductCreate = new Stripe.ProductCreateOptions
                    {
                        Name = linksCreateDto.ProductName,
                        Description = linksCreateDto.Description,
                        Images = linksCreateDto.ProductImage,
                        Metadata = new Dictionary<string, string>
                        {
                            { "price", $"{(long)Convert.ToDouble(linksCreateDto.PriceUnit)}" },
                            { "currency", linksCreateDto.Currency.ToLower() },
                        },
                    };
                Stripe.Product product = new Stripe.ProductService().Create(StripeOptionsProductCreate);
            
                var StripeOptionsPriceCreate = new Stripe.PriceCreateOptions
                    {
                        UnitAmount = (long)Convert.ToDouble(linksCreateDto.PriceUnit),
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
                            Quantity = 1,
                            AdjustableQuantity = new Stripe.PaymentLinkLineItemAdjustableQuantityOptions 
                            {
                                Enabled = false // can only order 1 by 1
                            }
                            // AdjustableQuantity = new Stripe.PaymentLinkLineItemAdjustableQuantityOptions
                            // {
                            //     Enabled = true,
                            //     Minimum = 1,
                            //     Maximum = (long)Convert.ToDouble(linksCreateDto.QuantityStock),
                            // }
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
                    AllowPromotionCodes = false, // Problem we CAN'T attach the coupon to a specific link .. so nop.
                    AutomaticTax =  new Stripe.PaymentLinkAutomaticTaxOptions {
                        Enabled = true,
                    },
                    AfterCompletion = new Stripe.PaymentLinkAfterCompletionOptions {
                        Type = "hosted_confirmation",
                        HostedConfirmation = new Stripe.PaymentLinkAfterCompletionHostedConfirmationOptions {
                            CustomMessage = "Thanks for your order!"
                        }
                    },
                    InvoiceCreation = new Stripe.PaymentLinkInvoiceCreationOptions {
                        Enabled = true
                    }
                };

                PaymentLink paymentLink = new Stripe.PaymentLinkService().Create(StripeOptionsPaymentLinkCreate);

                await _linkCollection.InsertOneAsync(new Link {
                    Name = linksCreateDto.Name,
                    GoogleOAuth2Sub = sub,
                    Iban = linksCreateDto.Iban,
                    PaymentUrl = paymentLink.Url,
                    StripeLinkId = paymentLink.Id,
                    StripeProductId = product.Id,
                    StripePriceId = price.Id
                });

                
                var result = await _linkCollection.FindAsync(l => l.StripeLinkId == paymentLink.Id);
                var justCreated = result.FirstOrDefault() ?? throw new Exception("Error during save link, can't find it in db");

                return new LinksDto {
                    Id = justCreated.Id!.ToString(),
                    Name = linksCreateDto.Name,
                    GoogleOAuth2Sub = sub,
                    Iban = linksCreateDto.Iban,
                    PaymentUrl = paymentLink.Url,
                    StripeProductId = product.Id,
                    StripeLinkId = paymentLink.Id,
                    StripePriceId = price.Id
                };
                
            } catch ( Exception e ) {
                _logger.LogError($"Error during save link for {linksCreateDto.Name} : {e.Message}");
                await session.CommitTransactionAsync();
                return null;

            }

        }

    }
    

    public async Task<List<Link>> GetLinksAsync(string sub, string idLast, int limit)
    {
        _logger.LogInformation($"GetLinks : {sub} for {idLast}");

        if (string.IsNullOrEmpty(sub)) {
            throw new ArgumentException("sub is empty");
        }

        var filter = Builders<Link>.Filter.Eq(l => l.GoogleOAuth2Sub, sub);

        if (!string.IsNullOrEmpty(idLast)) {
            var lastObjectId = new ObjectId(idLast);
            var idFilter = Builders<Link>.Filter.Gt("_id", lastObjectId);
            filter = Builders<Link>.Filter.And(filter, idFilter);
        }

        return await _linkCollection
            .Find(filter)
            .SortBy(l => l.Id)
            .Limit(limit)
            .ToListAsync();
    }

}