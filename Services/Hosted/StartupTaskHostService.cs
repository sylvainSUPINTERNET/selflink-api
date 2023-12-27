namespace Selflink_api.Services.Hosted;

using System.Threading;
using MongoDB.Bson;
using MongoDB.Driver;
using Selflink_api.Db.Models;

internal class StartupTaskHostService: IHostedService
{
    private readonly ILogger<StartupTaskHostService> _logger;
    private readonly IServiceProvider _serviceProvider;

    private readonly Db.IMongoDbClientSingleton _db;

    private readonly IMongoCollection<Link> _linkCollection;

    private readonly IMongoCollection<Order> _orderCollection;

    public StartupTaskHostService(ILogger<StartupTaskHostService> logger, IServiceProvider serviceProvider, Db.IMongoDbClientSingleton db)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _db = db;
        _linkCollection = db.GetDatabase().GetCollection<Link>(db.GetLinkCollectionName());
        _orderCollection = db.GetDatabase().GetCollection<Order>(db.GetOrderCollectionName());
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("StartupTaskHostService is starting.");

        _logger.LogInformation("Cleanup DB");

        await _linkCollection.DeleteManyAsync(new BsonDocument());
        await _orderCollection.DeleteManyAsync(new BsonDocument());

        var sub = "123";
        var productIdTest = "prod_test";

        var linkId = "link_test";

        List<Link> links = [];
        for (int i = 0; i < 5; i++)
        {
            var link = new Link
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = $"Link {i}",
                GoogleOAuth2Sub = sub,
                Iban = "FR7630006000011234567890189",
                PaymentUrl="https://www.google.com",
                StripeProductId = $"{productIdTest}{i}",
                StripePriceId = "http://test.com",
                StripeLinkId = linkId,
            };
            links.Add(link);
        }

        await _linkCollection.InsertManyAsync(links);

        List<Order> orders = [];

        for (int i = 0; i < 60; i++)
        {
            var order = new Order
            {
                Id = ObjectId.GenerateNewId().ToString(),
                StripeProductId = $"{productIdTest}1", // 60 orders for the first link
                StripePriceId = "http://test.com",
                StripePaymentIntentId = "http://test.com",
                Phone = "+33612345678",
                Email = "xxx@dl.com",
                ShippingLine1 = "1 rue de la paix",
                ShippingLine2 = "1 rue de la paix",
                ShippingCity = "Paris",
                ShippingPostalCode = "75000",
                ShippingCountry = "France",
                QuantityToSend = $"{i}",
                Amount = "1000",
                Currency = "eur",
                Status = "pending",
                GoogleOAuth2Sub = sub,

            };
            orders.Add(order);
        }


        var orderProductId0 = new Order
            {
                Id = ObjectId.GenerateNewId().ToString(),
                StripeProductId = $"{productIdTest}0", // 60 orders for the first link
                StripePriceId = "http://test.com",
                StripePaymentIntentId = "http://test.com",
                Phone = "+33612345678",
                Email = "xxx@dl.com",
                ShippingLine1 = "1 rue de la paix",
                ShippingLine2 = "1 rue de la paix",
                ShippingCity = "Paris",
                ShippingPostalCode = "75000",
                ShippingCountry = "France",
                QuantityToSend = $"{10}",
                Amount = "1000",
                Currency = "eur",
                Status = "pending",
                GoogleOAuth2Sub = sub,
            };
        orders.Add(orderProductId0);
        

        await _orderCollection.InsertManyAsync(orders);



    _logger.LogInformation("Inserted data with success !");

        // using (var scope = _serviceProvider.CreateScope())
        // {
        //     var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IScopedProcessingService>();

        //     await scopedProcessingService.DoWork(cancellationToken);
        // }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("StartupTaskHostService is stopping.");

        return Task.CompletedTask;
    }
}

