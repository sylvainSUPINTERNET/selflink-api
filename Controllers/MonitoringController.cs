
using Microsoft.AspNetCore.Mvc;

namespace Selflink_api.Controllers;

[ApiController]
[Route("monitor")]

public class MonitoringController : ControllerBase
{
    private readonly ILogger<MonitoringController> _logger;

    [HttpGet("health", Name = "Health")]
    public ActionResult Health ()
    {
        return Ok();
    }
}