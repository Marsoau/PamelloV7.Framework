using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.App;
using PamelloV7.Framework.Core.Logging;
using PamelloV7.Framework.Core.Repositories;

namespace PamelloV7.Framework.Repositories.Loaders;

public record PamelloRepositoryDescriptor(
    string ProviderName,
    Type ClassType,
    List<Type> Interfaces,
    List<Type> GenericInterfaces
);

public class PamelloRepositoriesLoader
{
    private readonly PamelloAppOptions _options;

    public readonly List<PamelloRepositoryDescriptor> RepositoriesDescriptors = [];
    
    public PamelloRepositoriesLoader(PamelloAppOptions options) {
        _options = options;
    }
    
    public void LoadRepositories() {
        var repositoriesTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t is { IsClass: true, IsAbstract: false } && t.IsAssignableTo(typeof(IPamelloRepository)));

        foreach (var repositoryType in repositoriesTypes) {
            RepositoriesDescriptors.Add(new PamelloRepositoryDescriptor(
                "",
                repositoryType,
                [],
                []
            ));
        }
    }

    public void Configure(IServiceCollection collection) {
        foreach (var repositoryType in RepositoriesDescriptors.Select(r => r.ClassType)) {
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
    
    public void LoadAndInitEntities(IServiceProvider services) {
        var repositories = RepositoriesDescriptors
            .Select(r => services.GetRequiredService(r.ClassType))
            .OfType<IPamelloDatabaseRepository>()
            .ToList();

        repositories.ForEach(repository => repository.LoadAll());
        repositories.ForEach(repository => repository.InitAll());
    }
}
