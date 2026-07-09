using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PamelloV7.Framework.Core.Logging;
using PamelloV7.Framework.Core.PEQL;
using PamelloV7.Framework.PEQL;
using PamelloV7.Framework.PEQL.Loaders;
using PamelloV7.Framework.Services.Loaders;
using PamelloV7.Framework.Shared.Entities.Containers;
using PamelloV7.Framework.Shared.Variants.Attributes;

namespace PamelloV7.Framework.App;

public partial class PamelloApp : IHost
{
    public readonly IHost Host;
    public readonly PamelloAppOptions Options;
    
    public IServiceProvider Services => Host.Services;
    
    public Action<IHost>? OnStartup;
    public Action<WebApplication>? OnApiStartup;
    
    public PamelloApp(IHost host, PamelloAppOptions options) {
        Host = host;
        
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
        OnStartup?.Invoke(Host);
        
        var repositoriesLoader = Services.GetRequiredService<PamelloEntityQueryLanguageLoader>();
        var servicesLoader = Services.GetRequiredService<PamelloServiceLoader>();
        
        var entityQueryService = Services.GetRequiredService<IPamelloEntityQueryService>();
        
        repositoriesLoader.LoadAllEntities(Services);
        servicesLoader.StartupServices(Services);
        
        SafeContainerGetters.GetById = (type, id) => entityQueryService.GetSingleById(type, id);
        
        if (Host is WebApplication asp && Options.UseApi) {
            StartupForApi(asp);
        }
        
        return Host.StartAsync(cancellationToken);
    }
    
    private void StartupForApi(WebApplication asp) {
        OnApiStartup?.Invoke(asp);
        
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
        return Host.StopAsync(cancellationToken);
    }
    
    public void Dispose() => Host.Dispose();
}
