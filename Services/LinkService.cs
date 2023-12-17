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
        this._logger.LogInformation("SaveLink");

        return new LinksDto
        {
            Id = "123",
            Name = "name"
        };
    }
}