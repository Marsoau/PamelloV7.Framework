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

            foreach (var interfaceType in serviceType.GetInterfaces()) {
                if (interfaceType == typeof(IPamelloService) ||
                    !interfaceType.IsAssignableTo(typeof(IPamelloService))
                ) continue;
                
                collection.AddSingleton(interfaceType, services => services.GetRequiredService(serviceType));
            }
        }
    }
    public void StartAppServices() {
        
    }
}
