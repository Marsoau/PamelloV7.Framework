using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PamelloV7.Framework.Config.Loaders;

namespace PamelloV7.Framework.App;

public class PamelloAppBuilder : IHostApplicationBuilder
{
    private readonly IHostApplicationBuilder _builder;
    
    public readonly PamelloAppOptions Options;
    
    public readonly PamelloConfigLoader ConfigLoader = new();
    
    public IDictionary<object, object> Properties => _builder.Properties;
    public IConfigurationManager Configuration => _builder.Configuration;
    public IHostEnvironment Environment => _builder.Environment;
    public ILoggingBuilder Logging => _builder.Logging;
    public IMetricsBuilder Metrics => _builder.Metrics;
    public IServiceCollection Services => _builder.Services;

    public PamelloAppBuilder(string[] args, PamelloAppOptions? options = null) {
        Options = options ?? new PamelloAppOptions();

        _builder = Options.UseApi
            ? WebApplication.CreateBuilder(args)
            : Host.CreateApplicationBuilder(args);
    }

    public void Configure() {
        Services.AddSingleton(Options);
        
        ConfigLoader.Load();
        
        if (_builder is WebApplicationBuilder webBuilder && Options.UseApi) {
            ConfigureForApi(webBuilder);
        }
        
        Logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Error);
    }

    private static void ConfigureForApi(WebApplicationBuilder webBuilder) {
        webBuilder.Services.AddControllers();
        webBuilder.Services.AddSignalR();
    }

    public PamelloApp Build() => _builder switch {
        WebApplicationBuilder webBuilder => new PamelloApp(webBuilder.Build(), Options),
        HostApplicationBuilder hostBuilder => new PamelloApp(hostBuilder.Build(), Options),
        _ => throw new InvalidOperationException("Unknown builder type")
    };

    public void ConfigureContainer<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory, Action<TContainerBuilder>? configure = null)
        where TContainerBuilder : notnull
        => _builder.ConfigureContainer(factory, configure);
}
