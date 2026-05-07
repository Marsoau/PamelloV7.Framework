using Microsoft.Extensions.DependencyInjection;

namespace PamelloV7.Framework.Core.Modules;

public abstract class PamelloModule
{
    public abstract string Name { get; }
    public abstract string Author { get; }
    public abstract string Description { get; }
    
    public virtual int Color => 0xFFFFFF;
    
    public virtual void Configure(IServiceCollection services) { }

    public virtual Task StartupAsync(IServiceProvider services) => Task.CompletedTask;
    public virtual Task StartedAsync(IServiceProvider services) => Task.CompletedTask;
    public virtual Task ShoutdownAsync() => Task.CompletedTask;
}
