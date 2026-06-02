using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Core.Actions;
using PamelloV7.Framework.Core.PEQL.Blocks;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.PEQL.Filters;

public abstract class PamelloQueryFilter(IServiceProvider services)
{
    protected readonly IServiceProvider Services = services;
    
    protected IPamelloEntityQueryService PEQL => Services.GetRequiredService<IPamelloEntityQueryService>();

    public IAsyncEnumerable<IPamelloBasicEntity> Entities {
        set; get => field ?? throw new InvalidOperationException("Filter entities not set");
    }

    public async IAsyncEnumerable<IPamelloBasicEntity> ExecuteByArgsAsync(string query, PamelloQueryBlock? arg) {
        var executeMethod = GetType().GetMethod("Execute")!;
        
        var peql = Services.GetRequiredService<IPamelloEntityQueryService>();
        
        var arguments = await PamelloStaticActions.EnumerateArgumentsForParameters(
            arg?.Text ?? "",
            executeMethod.GetParameters(),
            peql
        ).ToArrayAsync();

        await foreach (var entity in (IAsyncEnumerable<IPamelloBasicEntity>)executeMethod.Invoke(this, arguments)!) {
            yield return entity;
        };
    }
}
