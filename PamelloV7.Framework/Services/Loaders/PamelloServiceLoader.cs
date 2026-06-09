using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PamelloV7.Framework.App;
using PamelloV7.Framework.Core.Logging;
using PamelloV7.Framework.Core.Modules;
using PamelloV7.Framework.Core.Services.Attributes;
using PamelloV7.Framework.Core.Services.Base;
using PamelloV7.Framework.Core.Services.Loaders;
using PamelloV7.Framework.Extensions;
using PamelloV7.Framework.Shared.Extensions;

namespace PamelloV7.Framework.Services.Loaders;

public record AppServiceDescriptor(
    Type Type,
    Type[] OverridesTypes,
    Type[] Interfaces
);

public class PamelloServiceLoader : IPamelloServiceLoader
{
    private readonly PamelloAppOptions _options;

    public readonly List<List<Type>> AppServiceTypeChains = [];
    
    public PamelloServiceLoader(PamelloAppOptions options) {
        _options = options;
    }
    
    public void LoadAppServices() {
        var serviceTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectManyInNonModuleAssemblies()
            .Where(t => t is { IsClass: true, IsAbstract: false } && t.IsAssignableTo(typeof(IPamelloService)));
        
        PamelloOutput.Write($"Loading services:");
        
        foreach (var serviceType in serviceTypes) {
            var newChain = serviceType
                .GetAllBaseTypes(t => t.IsAssignableTo(typeof(IPamelloService)))
                .Prepend(serviceType)
                .ToList();
            
            //A -> B -> C -> D
            //C -> D
            
            int c, i, j;

            for (c = 0; c < AppServiceTypeChains.Count; c++) {
                var existingChain = AppServiceTypeChains[c];
                
                for (i = 0; i < existingChain.Count; i++) {
                    for (j = 0; j < newChain.Count; j++) {
                        if (existingChain[i] == newChain[j]) break;
                    }
                    if (j == newChain.Count) continue;

                    if (existingChain.Count - i != newChain.Count - j) {
                        throw new Exception($"Chains length is not compatible:\n{ChainStr(existingChain)}\n{ChainStr(newChain)}");
                    }

                    for (var o = 0; o < existingChain.Count - i; o++) {
                        if (existingChain[i + o] != newChain[j + o]) {
                            throw new Exception($"Chains are not compatible:\n{ChainStr(existingChain)}\n{ChainStr(newChain)}");
                        }
                    }

                    if (i > j) {
                        PamelloOutput.Write("New chain is shorter but compatible, skipping");
                        break;
                    }
                    
                    AppServiceTypeChains.Remove(existingChain);
                    AppServiceTypeChains.Add(newChain);

                    PamelloOutput.Write($"Replaced: from {ChainStr(existingChain)} to {ChainStr(newChain)}");
                    
                    break;
                }
                
                if (i != existingChain.Count) break;
            }
            
            if (c != AppServiceTypeChains.Count) continue;

            PamelloOutput.Write($"Added: {ChainStr(newChain)}");
            AppServiceTypeChains.Add(newChain);
        }

        PamelloOutput.Write($"{AppServiceTypeChains.Count} service chains loaded");
        
        return;
        
        string ChainStr(List<Type> chain) {
            return $"[{string.Join(" -> ", chain.Select(t => t.Name))}]";
        }
    }

    public void ConfigureAppServices(IServiceCollection collection) {
        foreach (var chain in AppServiceTypeChains) {
            for (var i = 0; i < chain.Count; i++) {
                var serviceType = chain[i];
                var previousServiceType = i > 0 ? chain[i - 1] : null;

                TryAddService(serviceType, previousServiceType);
                
                foreach (var interfaceType in serviceType.GetInterfaces()) {
                    if (interfaceType == typeof(IPamelloService) ||
                        !interfaceType.IsAssignableTo(typeof(IPamelloService))
                    ) continue;
                
                    TryAddService(interfaceType, serviceType);
                }
            }
        }
        
        return;
        
        void TryAddService(Type serviceType, Type? actualServiceType) {
            var isTransient = serviceType.GetCustomAttribute<PamelloTransientServiceAttribute>() is not null;

            PamelloOutput.Write($"{(isTransient ? "Transient" : "Singleton")}: {serviceType.Name}{(
                actualServiceType is not null ? $" -> {actualServiceType.Name}" : ""
            )}");

            if (isTransient) {
                if (actualServiceType is null) {
                    collection.TryAddTransient(serviceType);
                }
                else {
                    collection.TryAddTransient(serviceType, services => services.GetRequiredService(actualServiceType));
                }
            }
            else {
                if (actualServiceType is null) {
                    collection.TryAddSingleton(serviceType);
                }
                else {
                    collection.TryAddSingleton(serviceType, services => services.GetRequiredService(actualServiceType));
                }
            }
        }
    }
    
    public void StartupServices(IServiceProvider services) {
        var typesWithStartup = AppServiceTypeChains.Select(c => c.First()).Where(t => {
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
