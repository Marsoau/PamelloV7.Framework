using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PamelloV7.Framework.Core.Logging;
using PamelloV7.Framework.Shared.Variants.Attributes;

namespace PamelloV7.Framework.App;

public partial class PamelloApp : IHost
{
    private readonly IHost _host;
    public readonly PamelloAppOptions Options;
    
    public IServiceProvider Services => _host.Services;
    
    public PamelloApp(IHost host, PamelloAppOptions options) {
        _host = host;
        
        Options = options;
    }
    
    private static PamelloAppOptions GetDefaultOptions() => new();

    public static PamelloAppBuilder CreateBuilder(
        string[] args,
        [Variant(nameof(GetDefaultOptions))]
        PamelloAppOptions? options,
        Action<PamelloAppOptions>? editOptionsAfterConfig = null
    ) {
        var builder = new PamelloAppBuilder(args, options);
        builder.Configure(editOptionsAfterConfig);

        return builder;
    }
    
    public Task StartAsync(CancellationToken cancellationToken = default) {
        if (_host is WebApplication asp && Options.UseApi) {
            StartupForApi(asp);
        }
        
        return _host.StartAsync(cancellationToken);
    }
    
    private static void StartupForApi(WebApplication asp) {
        //asp.MapHub<SignalHub>("/Signal");

        asp.Lifetime.ApplicationStarted.Register(() => {
            var server = asp.Services.GetRequiredService<IServer>();
            var addresses = server.Features.Get<IServerAddressesFeature>()?.Addresses ?? [];
            
            foreach (var address in addresses)
                PamelloOutput.Write($"Listening on: {address}");
        });
        
        asp.MapControllers();
    }
    
    public Task StopAsync(CancellationToken cancellationToken = default) {
        return _host.StopAsync(cancellationToken);
    }
    
    public void Dispose() => _host.Dispose();
}
