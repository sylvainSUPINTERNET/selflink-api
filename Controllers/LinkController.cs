using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    private readonly ILinkService linkService;

    public LinkController(ILogger<LinkController> logger, ILinkService linkService)
    {
        _logger = logger;
        this.linkService = linkService;
    }

    [HttpPost(Name = "CreateLinkAsync")]
    public async Task<ActionResult<Link>> SaveAsync( LinksCreateDto linkCreateDto )
    {
        LinksDto? res = await linkService.SaveLink(linkCreateDto);

        if ( res == null ) {
            return BadRequest();
        }

        return Ok(res);
    }


    [HttpGet(Name = "GetLinkAsync")]
    public async Task<ActionResult<Link>> GetListAsync () 
    {
        // Flow : 

        // j'envois X classé en ASC. 
        // liste de X elements 

        // côté front
        // j'affiche les X elements quand j'arrive à la fin de la liste je fais une requete pour avoir les X suivant ( envoit l'_id du dernier du coup )
        
        
        var result = await linkService.GetLinksAsync("123");
        return Ok(result);
    }
}
