using Microsoft.AspNetCore.Mvc;
using PamelloV7.Framework.Core.Controllers;
using PamelloV7.Framework.Core.Services.Attributes;
using PamelloV7.Framework.Core.Services.Base;

namespace PamelloV7.Framework.Controllers;

[PamelloTransientService]

[Route("[controller]")]
[ApiController]
public class PingController : PamelloControllerBase, IPamelloService
{
    public PingController(IServiceProvider services) : base(services) { }
    
    protected virtual string Message => "Pong";
    
    [HttpGet("{*ignored}")]
    public IActionResult Get() {
        var user = Authorization.GetUser();
        
        return Ok(Message);
    }
}
