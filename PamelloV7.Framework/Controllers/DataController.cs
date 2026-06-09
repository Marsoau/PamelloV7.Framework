using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Core.Controllers;
using PamelloV7.Framework.Core.PEQL;
using PamelloV7.Framework.Core.Scope;
using PamelloV7.Framework.Shared.Exceptions;

namespace PamelloV7.Framework.Controllers;

[ApiController]
[Route("[controller]")]
public class DataController : PamelloControllerBase
{
    public DataController(IServiceProvider services) : base(services) { }
    
    [HttpGet("{**query}")]
    public async Task<IActionResult> Get(string query) {
        var user = Authorization.GetUser();
        
        ObjectResult? result = null;
        
        await PamelloAppScope.SetUserIn(user, async () =>
            result = Ok(await Query(query).ToListAsync())
        );
        
        if (result is null) throw new PamelloException("Query should not be null here");
        
        return result;
    }

    public IAsyncEnumerable<object?> Query(string query) {
        var peql = Services.GetRequiredService<IPamelloEntityQueryService>();
        return peql.GetAsync(query);
    }
}
