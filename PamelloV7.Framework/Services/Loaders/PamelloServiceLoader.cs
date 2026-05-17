using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.App;
using PamelloV7.Framework.Core.Logging;
using PamelloV7.Framework.Core.Modules;
using PamelloV7.Framework.Core.Services.Base;
using PamelloV7.Framework.Core.Services.Loaders;
using PamelloV7.Framework.Extensions;

namespace PamelloV7.Framework.Services.Loaders;

public class PamelloServiceLoader : IPamelloServiceLoader
{
    private readonly PamelloAppOptions _options;

    public readonly List<Type> AppServiceTypes = [];
    
    public PamelloServiceLoader(PamelloAppOptions options) {
        _options = options;
    }
    
    public void LoadAppServices() {
        var serviceTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectManyInNonModuleAssemblies()
            .Where(t => t is { IsClass: true, IsAbstract: false } && t.IsAssignableTo(typeof(IPamelloService)));
        
        AppServiceTypes.AddRange(serviceTypes);
    }

    public void ConfigureAppServices(IServiceCollection collection) {
        foreach (var serviceType in AppServiceTypes) {
            collection.AddSingleton(serviceType);

            var baseType = serviceType.BaseType;
            while (baseType is not null && baseType != typeof(object)) {
                if (!baseType.IsAssignableTo(typeof(IPamelloService))) break;
                
                collection.AddSingleton(baseType, services => services.GetRequiredService(serviceType));
                baseType = baseType.BaseType;
            }
            
            foreach (var interfaceType in serviceType.GetInterfaces()) {
                if (interfaceType == typeof(IPamelloService) ||
                    !interfaceType.IsAssignableTo(typeof(IPamelloService))
                ) continue;
                
                collection.AddSingleton(interfaceType, services => services.GetRequiredService(serviceType));
            }
        }
    }
    public void StartupServices(IServiceProvider services) {
        var typesWithStartup = AppServiceTypes.Where(t => {
            var type = t;
            while (type is not null && type != typeof(object) && type != typeof(IPamelloService)) {
                if (type.GetMethod("Startup") is not null) return true;
                type = type.BaseType;
            }
            
            return false;
        });

        PamelloOutput.Write("Starting services:");
        foreach (var type in typesWithStartup) {
            PamelloOutput.Write($"| {type.Name}");
            type.GetMethod("Startup")?.Invoke(services.GetRequiredService(type), [services]);
        }
    }
}
