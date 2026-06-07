using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Core.PEQL;

namespace PamelloV7.Framework.Core.Actions;

public abstract class PamelloBasicActions
{
    public IServiceProvider Services { get; private set; } = null!;
    
    public IPamelloEntityQueryService PEQL => field ??= Services.GetRequiredService<IPamelloEntityQueryService>();
    
    public virtual void InitializeActions(IServiceProvider services) {
        Services = services;
    }
}
