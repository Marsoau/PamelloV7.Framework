namespace PamelloV7.Framework.Core.Services.Base;

public interface IPamelloService : IDisposable
{
    public void Startup(IServiceProvider services) { }
    public void Shutdown() { }
    
    void IDisposable.Dispose() => Dispose();
    public new void Dispose() { }
}
