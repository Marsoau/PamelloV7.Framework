using Microsoft.AspNetCore.Mvc;

namespace PamelloV7.Framework.Controllers;

[Route("[controller]")]
[ApiController]
public class PingController : ControllerBase
{
    protected virtual string Message => "Pong";
    
    [HttpGet("{*ignored}")]
    public IActionResult Get() {
        return Ok(Message);
    }
}