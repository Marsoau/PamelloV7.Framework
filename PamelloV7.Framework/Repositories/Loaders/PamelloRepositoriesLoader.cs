using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.App;
using PamelloV7.Framework.Core.Logging;
using PamelloV7.Framework.Core.Repositories;
using PamelloV7.Framework.Core.Repositories.Attributes;

namespace PamelloV7.Framework.Repositories.Loaders;

public record PamelloRepositoryDescriptor(
    IPamelloRepositoryAttribute Attribute,
    Type RepositoryType,
    List<Type> Interfaces,
    List<Type> GenericInterfaces
)
{
    public bool IsDatabaseRepository => DatabaseAttribute is not null;
    public IPamelloDatabaseRepositoryAttribute? DatabaseAttribute => Attribute as IPamelloDatabaseRepositoryAttribute;

    public Type? EntityDboType => Attribute.EntityType.GetNestedType("Dbo");
    public Type? EntityDtoType => Attribute.EntityType.GetNestedType("Dto");
};

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
            var repositoryAttribute = repositoryType.GetCustomAttributes()
                .OfType<IPamelloRepositoryAttribute>()
                .FirstOrDefault();
            
            if (repositoryAttribute is null) continue;
            
            RepositoriesDescriptors.Add(new PamelloRepositoryDescriptor(
                repositoryAttribute,
                repositoryType,
                [],
                []
            ));
        }
    }

    public void Configure(IServiceCollection collection) {
        foreach (var repositoryType in RepositoriesDescriptors.Select(r => r.RepositoryType)) {
            collection.AddSingleton(repositoryType);

            PamelloOutput.Write($"Adding {repositoryType.Name}");
            foreach (var interfaceType in repositoryType.GetInterfaces()) {
                if (!interfaceType.IsAssignableTo(typeof(IPamelloRepository))) continue;
                if (interfaceType == typeof(IPamelloRepository) ||
                    interfaceType == typeof(IPamelloDatabaseRepository)
                ) continue;
                
                PamelloOutput.Write($"| {interfaceType.Name}");
                
                collection.AddSingleton(interfaceType, services => services.GetRequiredService(repositoryType));
            }
        }
    }
    
    public void LoadAllEntities(IServiceProvider services) {
        var repositories = RepositoriesDescriptors
            .Select(r => services.GetRequiredService(r.RepositoryType))
            .OfType<IPamelloDatabaseRepository>()
            .ToList();

        repositories.ForEach(repository => repository.LoadAll());
    }
}
