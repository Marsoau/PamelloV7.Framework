using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.App;
using PamelloV7.Framework.Core.Logging;
using PamelloV7.Framework.Core.Repositories;

namespace PamelloV7.Framework.Repositories.Loaders;

public class PamelloRepositoriesLoader
{
    private readonly PamelloAppOptions _options;

    public readonly List<Type> RepositoriesTypes = [];
    
    public PamelloRepositoriesLoader(PamelloAppOptions options) {
        _options = options;
    }
    
    public void Load() {
        var repositoriesTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t is { IsClass: true, IsAbstract: false } && t.IsAssignableTo(typeof(IPamelloRepository)));
        
        RepositoriesTypes.AddRange(repositoriesTypes);
    }

    public void Configure(IServiceCollection collection) {
        foreach (var repositoryType in RepositoriesTypes) {
            collection.AddSingleton(repositoryType);

            PamelloOutput.Write($"Adding {repositoryType.Name}");
            foreach (var interfaceType in repositoryType.GetInterfaces()) {
                if (!interfaceType.IsAssignableTo(typeof(IPamelloRepository))) continue;
                if (interfaceType == typeof(IPamelloRepository) ||
                    interfaceType == typeof(IPamelloLazyDatabaseRepository) ||
                    interfaceType == typeof(IPamelloDatabaseRepository)
                ) continue;
                
                PamelloOutput.Write($"| {interfaceType.Name}");
                
                collection.AddSingleton(interfaceType, services => services.GetRequiredService(repositoryType));
            }
        }
    }
}
