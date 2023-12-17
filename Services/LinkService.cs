using Selflink_api.Dto;

namespace Selflink_api.Services;

public class LinkService : ILinkService
{
    private readonly ILogger<LinkService> _logger;

    public LinkService(ILogger<LinkService> logger) {
        
        _logger = logger;
    }

    public LinksDto SaveLink()
    {
        _logger.LogInformation("SaveLink");

        // TODO: https://github.com/sylvainSUPINTERNET/zerecruteur-service/blob/master/src/services/product.service.ts
        // TODO: difference c'est qu'il faut utiliser sub du claims pour identifier l'utilisateur !
        return new LinksDto
        {
            Id = "123",
            Name = "name"
        };
    }
}