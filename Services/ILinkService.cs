using Selflink_api.Db.Models;
using Selflink_api.Dto;

namespace Selflink_api.Services
{
    public interface ILinkService
    {
        public Task<LinksDto?> SaveLink(LinksCreateDto linksCreateDto);

        public Task<List<Link>> GetLinksAsync(string sub);
    }
}