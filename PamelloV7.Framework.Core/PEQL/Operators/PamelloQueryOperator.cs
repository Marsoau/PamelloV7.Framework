using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Core.PEQL.Blocks;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.PEQL.Operators;

public abstract class PamelloQueryOperator
{
    protected readonly IServiceProvider Services;
    
    protected IPamelloEntityQueryService PEQL => Services.GetRequiredService<IPamelloEntityQueryService>();

    public PamelloQueryOperator(IServiceProvider services) {
        Services = services;
    }

    
    public abstract IAsyncEnumerable<IPamelloBasicEntity> Execute(string query, PamelloQueryBlock arg);
}

