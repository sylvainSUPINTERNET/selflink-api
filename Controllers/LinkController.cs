using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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
[Authorize]
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
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var userIssuer = User.FindFirst("iss")?.Value;

        _logger.LogInformation($"sub: {userId}");
        _logger.LogInformation($"userEmail: {userEmail}");
        _logger.LogInformation($"iss: {userIssuer}");

        if ( userId == null) 
        {   
            _logger.LogError("userId is null");
            return BadRequest();
        }

        if ( userEmail == null ) 
        {   
            _logger.LogError("userEmail is null");
            return BadRequest();
        }

        if ( userIssuer == null ) 
        {   
            _logger.LogError("userIssuer is null");
            return BadRequest();
        }

        LinksDto? res = await linkService.SaveLink(linkCreateDto, userEmail, userId, userIssuer);

        if ( res == null ) {
            return BadRequest();
        }

        return Ok(res);
    }


    [HttpGet(Name = "GetLinkAsync")]
    public async Task<ActionResult<Link>> GetListAsync ()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if ( userId == null) 
        {   
            _logger.LogError("userId is null");
            return BadRequest();
        }

        try {
            var result = await linkService.GetLinksAsync(userId);
            return Ok(result);

        } catch ( Exception e ) {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("deactivate", Name="DeactivateLink")]
    public async Task<ActionResult<bool>> DeactivateLinkAsync(LinksDeactivateDto linksDeactivateDto)
    {

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if ( userId == null) 
        {   
            _logger.LogError("userId is null");
            return BadRequest();
        }
    
        try {
            await linkService.DeactivateLinkAsync(linksDeactivateDto.PaymentLinkId, userId);
            return Ok();
        } catch ( Exception e ) {
            return BadRequest(e.Message);
        }
    }


}
