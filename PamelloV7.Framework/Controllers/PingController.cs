using Microsoft.AspNetCore.Mvc;
using PamelloV7.Framework.Core.Services.Attributes;
using PamelloV7.Framework.Core.Services.Base;

namespace PamelloV7.Framework.Controllers;

[PamelloTransientService]

[Route("[controller]")]
[ApiController]
public class PingController : ControllerBase, IPamelloService
{
    protected virtual string Message => "Pong";
    
    [HttpGet("{*ignored}")]
    public IActionResult Get() {
        return Ok(Message);
    }
}

public class AnotherPingController : PingController
{
    override protected string Message => "Another Pong";
}