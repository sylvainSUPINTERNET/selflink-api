using Selflink_api.Dto;

namespace Selflink_api.Services
{
    public interface ILinkService
    {
        public Task<LinksDto?> SaveLink(LinksCreateDto linksCreateDto);
    }
}