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
    

    public async Task<List<Link>> GetLinksAsync(string sub)
    {
        _logger.LogInformation($"GetLinks : {sub}");

        var lastId = "65889cd13938796125d6a5a4";

        // var sort = Builders<BsonDocument>.Sort.Descending("_id");
        // var links = await _db.Links.Where( link => link.GoogleOAuth2Sub == sub );


        // var links = await _db.GetClient()
        //             //    .Where(l => l.GoogleOAuth2Sub == "123" && l.Id > objectIdToCompare)
        //             .Where(l=>l.GoogleOAuth2Sub == "123")
        //                .OrderBy(l => l.Id)
        //                .Take(2)
        //                .ToListAsync();

        // request 1
        // var result = await _linkCollection
        //     .Find(l => l.GoogleOAuth2Sub == sub)
        //     .SortBy(l => l.Id)
        //     .Limit(2)
        //     .ToListAsync();

        // Request 2 ( sending previous _id )
        var filter = Builders<Link>.Filter.And(
            Builders<Link>.Filter.Eq(l => l.GoogleOAuth2Sub, sub),
            Builders<Link>.Filter.Gt("_id", new ObjectId(lastId))
        );

        var result = await _linkCollection
            .Find(filter)
            .SortBy(l => l.Id)
            // .Find(l=>l.GoogleOAuth2Sub == sub)
            .Limit(2)
            .ToListAsync();

        return result;
    }

}