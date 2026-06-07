using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.App;
using PamelloV7.Framework.Core.Logging;
using PamelloV7.Framework.Core.PEQL.Attributes;
using PamelloV7.Framework.Core.Repositories;
using PamelloV7.Framework.Core.Repositories.Attributes;
using PamelloV7.Framework.PEQL.Descriptors;

namespace PamelloV7.Framework.PEQL.Loaders;

public record PamelloRepositoryDescriptor(
    IPamelloRepositoryAttribute Attribute,
    Type RepositoryType
)
{
    public bool IsDatabaseRepository => DatabaseAttribute is not null;
    public IPamelloDatabaseRepositoryAttribute? DatabaseAttribute => Attribute as IPamelloDatabaseRepositoryAttribute;

    public Type? EntityDaoType => Attribute.EntityType.GetNestedType("Dao");
    public Type? EntityDtoType => Attribute.EntityType.GetNestedType("Dto");
};

public class PamelloEntityQueryLanguageLoader
{
    private readonly PamelloAppOptions _options;

    public readonly List<Type> RepositoriesTypesToDrop = [];
    public readonly List<PamelloRepositoryDescriptor> RepositoriesDescriptors = [];
    
    public readonly List<PamelloQueryOperatorDescriptor> OperatorsDescriptors = [];
    public readonly List<PamelloQueryFilterDescriptor> FiltersDescriptors = [];
    
    public PamelloEntityQueryLanguageLoader(PamelloAppOptions options) {
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
                repositoryType
            ));
        }
    }

    public void LoadOperators() {
        PamelloOutput.Write("Loading operators");
        var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes())
            .Where(t => t.GetCustomAttributes().Any(a => a is PamelloQueryOperatorAttribute));
        
        foreach (var type in types) {
            var attribute = type.GetCustomAttribute<PamelloQueryOperatorAttribute>();
            if (attribute is null) continue;

            PamelloOutput.Write($"| {attribute.Operator} {attribute.Name}");
            if (attribute.Description is not null)
                PamelloOutput.Write($"|   {attribute.Description}");
            
            OperatorsDescriptors.Add(new PamelloQueryOperatorDescriptor(
                attribute,
                type
            ));
        }
    }
    
    public void LoadFilters() {
        PamelloOutput.Write("Loading filters");
        var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes())
            .Where(t => t.GetCustomAttributes().Any(a => a is PamelloQueryFilterAttribute));
        
        foreach (var type in types) {
            var attribute = type.GetCustomAttribute<PamelloQueryFilterAttribute>();
            if (attribute is null) continue;

            PamelloOutput.Write($"| {attribute}");
            if (attribute.Description is not null)
                PamelloOutput.Write($"|   {attribute.Description}");
            
            FiltersDescriptors.Add(new PamelloQueryFilterDescriptor(
                attribute,
                type
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
        
        foreach (var operatorDescriptor in OperatorsDescriptors) {
            collection.AddTransient(operatorDescriptor.Type);
        }
        foreach (var filterDescriptor in FiltersDescriptors) {
            collection.AddTransient(filterDescriptor.Type);
        }
    }

    public void RegisterRepositoryToDrop(Type repositoryType) {
        if (RepositoriesTypesToDrop.Contains(repositoryType)) return;
        
        RepositoriesTypesToDrop.Add(repositoryType);
    }
    
    public void LoadAllEntities(IServiceProvider services) {
        var repositories = RepositoriesDescriptors
            .Select(r => services.GetRequiredService(r.RepositoryType))
            .OfType<IPamelloDatabaseRepository>()
            .ToList();

        var repositoriesToDrop = RepositoriesTypesToDrop.Select(services.GetServices).SelectMany(s => s).OfType<IPamelloDatabaseRepository>().ToList();
        
        foreach (var repository in repositoriesToDrop) {
            var collection = repository.GetCollection();
            collection.Drop();
        }

        repositories.ForEach(repository => repository.LoadAll());
    }
}
