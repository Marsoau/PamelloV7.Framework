using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PamelloV7.Framework.App;

public class PamelloApp : IHost
{
    private readonly IHost _host;
    
    public IServiceProvider Services => _host.Services;
    
    public PamelloApp(IHost host) {
        _host = host;
    }

    public static PamelloAppBuilder CreateBuilder(string[] args, PamelloAppOptions? options = null) {
        var builder = new PamelloAppBuilder(args, options);
        builder.Configure();

        return builder;
    }
    
    public Task StartAsync(CancellationToken cancellationToken = default) {
        Console.WriteLine("Pamello Start");
        
        return _host.StartAsync(cancellationToken);
    }
    
    public Task StopAsync(CancellationToken cancellationToken = default) {
        Console.WriteLine("Pamello Stop");
        return _host.StopAsync(cancellationToken);
    }
    
    public void Dispose() => _host.Dispose();
}
