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
using PamelloV7.Framework.Core.Context;
using PamelloV7.Framework.Core.Logging;
using PamelloV7.Framework.Core.Modules.Loaders;
using PamelloV7.Framework.PEQL.Filters;
using PamelloV7.Framework.PEQL.Loaders;
using PamelloV7.Framework.PEQL.Operators;
using PamelloV7.Framework.Services.Loaders;

namespace PamelloV7.Framework.App;

public class PamelloAppBuilder : IHostApplicationBuilder
{
    private readonly IHostApplicationBuilder _builder;
    
    public readonly PamelloAppOptions Options;
    
    public readonly PamelloConfigLoader ConfigLoader;
    public readonly PamelloServiceLoader ServiceLoader;
    public readonly PamelloEntityQueryLanguageLoader EntityQueryLanguageLoader;
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
        EntityQueryLanguageLoader = new PamelloEntityQueryLanguageLoader(Options);
        ModuleLoader = null!;

        _builder = Options.UseApi
            ? WebApplication.CreateBuilder(args)
            : Host.CreateApplicationBuilder(args);
    }

    public void Configure(Action<PamelloAppOptions>? editOptionsAfterConfig = null) {
        Services.AddSingleton(Options);
        
        Services.AddTransient<PamelloQueryTestFilter>();
        
        Services.AddSingleton(ConfigLoader);
        Services.AddSingleton(ServiceLoader);
        Services.AddSingleton(EntityQueryLanguageLoader);

        Services.AddSingleton(Services);
        
        ConfigLoader.Load();
        ConfigLoader.FinishBeforeModules();
        
        editOptionsAfterConfig?.Invoke(Options);
        
        Options.Logger?.Modules = ModuleLoader;
        PamelloOutput.Logger = Options.Logger;
        
        ServiceLoader.LoadAppServices();
        ServiceLoader.ConfigureAppServices(Services);
        
        //load modules here later
        
        EntityQueryLanguageLoader.LoadRepositories();
        EntityQueryLanguageLoader.LoadOperators();
        EntityQueryLanguageLoader.LoadFilters();
        
        EntityQueryLanguageLoader.Configure(Services);

        if (_builder is WebApplicationBuilder webBuilder && Options.UseApi) {
            ConfigureForApi(webBuilder);
        }
        
        Logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Error);
        Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Error);
    }

    private void ConfigureForApi(WebApplicationBuilder webBuilder) {
        webBuilder.Services.AddControllers()
            .AddControllersAsServices();
        webBuilder.Services.AddSignalR();

        if (Options.ApiUrls.Count > 0)
            webBuilder.WebHost.UseUrls([..Options.ApiUrls]);
    }

    public PamelloApp Build() {
        var app = _builder switch {
            WebApplicationBuilder webBuilder => new PamelloApp(webBuilder.Build(), Options),
            HostApplicationBuilder hostBuilder => new PamelloApp(hostBuilder.Build(), Options),
            _ => throw new InvalidOperationException("Unknown builder type")
        };
        
        PamelloAppContext.Services = app.Services;
        
        return app;
    }

    public void ConfigureContainer<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory, Action<TContainerBuilder>? configure = null)
        where TContainerBuilder : notnull
        => _builder.ConfigureContainer(factory, configure);
}
