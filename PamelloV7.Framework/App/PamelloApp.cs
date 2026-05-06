using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PamelloV7.Framework.Core.Logging;

namespace PamelloV7.Framework.App;

public class PamelloApp : IHost
{
    private readonly IHost _host;
    public readonly PamelloAppOptions Options;
    
    public IServiceProvider Services => _host.Services;
    
    public PamelloApp(IHost host, PamelloAppOptions options) {
        _host = host;
        
        Options = options;
    }

    public static PamelloAppBuilder CreateBuilder(string[] args, PamelloAppOptions? options = null) {
        var builder = new PamelloAppBuilder(args, options);
        builder.Configure();

        return builder;
    }
    
    public Task StartAsync(CancellationToken cancellationToken = default) {
        PamelloOutput.Logger = Options.Logger;
        
        PamelloOutput.Write("Start");
        
        return _host.StartAsync(cancellationToken);
    }
    
    public Task StopAsync(CancellationToken cancellationToken = default) {
        PamelloOutput.Write("Stop");
        return _host.StopAsync(cancellationToken);
    }
    
    public void Dispose() => _host.Dispose();
}
