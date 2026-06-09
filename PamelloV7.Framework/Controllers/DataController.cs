using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Core.Controllers;
using PamelloV7.Framework.Core.Exceptions;
using PamelloV7.Framework.Core.PEQL;
using PamelloV7.Framework.Core.Scope;
using PamelloV7.Framework.Shared.Exceptions;

namespace PamelloV7.Framework.Controllers;

[ApiController]
[Route("[controller]")]
public class DataController : PamelloControllerBase
{
    public DataController(IServiceProvider services) : base(services) { }
    
    public virtual bool IsUserRequired => true;
    
    [HttpGet("{**query}")]
    public async Task<IActionResult> Get(string query) {
        var user = ControllerAuthorization.GetUser(Request);
        if (user is null && IsUserRequired) throw new PamelloNoScopeUserException();

        query = $"{query}{Request.QueryString}";

        Console.WriteLine($"Query: {query}");
        
        return await PamelloAppScope.SetUserIn(user, async () =>
            Ok(await Query(query).ToListAsync())
        );
    }

    public IAsyncEnumerable<object?> Query(string query) {
        var peql = Services.GetRequiredService<IPamelloEntityQueryService>();
        return peql.GetAsync(query);
    }
}
