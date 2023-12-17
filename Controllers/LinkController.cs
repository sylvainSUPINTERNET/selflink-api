using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.Linq;
using Selflink_api.Db;
using Selflink_api.Db.Models;
using Selflink_api.Dto;
using Selflink_api.Services;

namespace Selflink_api.Controllers;

[ApiController]
[Route("api/links")]
public class LinkController : ControllerBase
{
    private readonly ILogger<LinkController> _logger;


    private readonly SelflinkDbContext db;

    private readonly ILinkService linkService;

    public LinkController(ILogger<LinkController> logger, SelflinkDbContext db, ILinkService linkService)
    {
        _logger = logger;
        this.db = db;
        this.linkService = linkService;
    }

    [HttpGet(Name = "CreateLinkAsync")]
    public async Task<ActionResult<Link>> SaveAsync()
    {
        linkService.SaveLink();
        
        Link link = new Link { Name = "linke" };
        db.Links.Add(link);
        await db.SaveChangesAsync();

        var t = await db.Links.Where(x => x.Name == "linke").FirstOrDefaultAsync();
        
       
        return Ok(new LinksDto {
            Id = t!.Id.ToString(),
            Name = t.Name
        });
    }
}
