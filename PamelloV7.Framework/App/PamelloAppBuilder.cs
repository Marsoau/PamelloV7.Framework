using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PamelloV7.Framework.Config.Loaders;
using PamelloV7.Framework.Core.Config.Attributes;
using PamelloV7.Framework.Core.Logging;
using PamelloV7.Framework.Core.Modules.Loaders;
using PamelloV7.Framework.Repositories.Loaders;
using PamelloV7.Framework.Services.Loaders;

namespace PamelloV7.Framework.App;

public class PamelloAppBuilder : IHostApplicationBuilder
{
    private readonly IHostApplicationBuilder _builder;
    
    public readonly PamelloAppOptions Options;
    
    public readonly PamelloConfigLoader ConfigLoader;
    public readonly PamelloServiceLoader ServiceLoader;
    public readonly PamelloRepositoriesLoader RepositoriesLoader;
    public readonly IPamelloModuleLoader ModuleLoader;
    
    public IDictionary<object, object> Properties => _builder.Properties;
    public IConfigurationManager Configuration => _builder.Configuration;
    public IHostEnvironment Environment => _builder.Environment;
    public ILoggingBuilder Logging => _builder.Logging;
    public IMetricsBuilder Metrics => _builder.Metrics;
    public IServiceCollection Services => _builder.Services;

    public PamelloAppBuilder(string[] args, PamelloAppOptions? options = null) {
        Options = options ?? new PamelloAppOptions();
        
        ConfigLoader = new PamelloConfigLoader(Options);
        ServiceLoader = new PamelloServiceLoader(Options);
        RepositoriesLoader = new PamelloRepositoriesLoader(Options);
        ModuleLoader = null!;

        _builder = Options.UseApi
            ? WebApplication.CreateBuilder(args)
            : Host.CreateApplicationBuilder(args);
    }

    class SomeAAndBClass : InterfaceA, InterfaceB;
    interface InterfaceA;
    interface InterfaceB;

    public void Configure(Action<PamelloAppOptions>? editOptionsAfterConfig = null) {
        Services.AddSingleton(Options);
        
        Services.AddSingleton(ConfigLoader);
        Services.AddSingleton(ServiceLoader);
        Services.AddSingleton(RepositoriesLoader);

        Services.AddSingleton(Services);
        
        Services.AddSingleton<InterfaceA, SomeAAndBClass>();
        Services.AddSingleton<InterfaceB, SomeAAndBClass>();
        
        ConfigLoader.Load();
        ConfigLoader.FinishBeforeModules();
        
        editOptionsAfterConfig?.Invoke(Options);
        
        Options.Logger?.Modules = ModuleLoader;
        PamelloOutput.Logger = Options.Logger;
        
        ServiceLoader.LoadAppServices();
        ServiceLoader.ConfigureAppServices(Services);
        
        //load modules here later
        
        RepositoriesLoader.LoadRepositories();
        RepositoriesLoader.Configure(Services);

        if (_builder is WebApplicationBuilder webBuilder && Options.UseApi) {
            ConfigureForApi(webBuilder);
        }
        
        Logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Error);
        Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Error);
    }

    private void ConfigureForApi(WebApplicationBuilder webBuilder) {
        webBuilder.Services.AddControllers();
        webBuilder.Services.AddSignalR();

        if (Options.ApiUrls.Count > 0)
            webBuilder.WebHost.UseUrls([..Options.ApiUrls]);
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
