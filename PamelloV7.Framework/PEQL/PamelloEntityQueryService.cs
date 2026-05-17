using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Core.Logging;
using PamelloV7.Framework.Core.PEQL;
using PamelloV7.Framework.Core.Repositories;
using PamelloV7.Framework.Repositories.Loaders;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.PEQL;

public record PamelloQueryProviderDescriptor(
    string Name,
    Type EntityType,
    IPamelloRepository Repository
);

public class PamelloEntityQueryService : IPamelloEntityQueryService
{
    private readonly IServiceProvider _services;
    
    private readonly PamelloRepositoriesLoader _repositoriesLoader;
    
    public readonly List<PamelloQueryProviderDescriptor> Providers = [];
    
    public PamelloEntityQueryService(IServiceProvider services) {
        _services = services;
        
        _repositoriesLoader = services.GetRequiredService<PamelloRepositoriesLoader>();
    }

    public void Startup(IServiceProvider services) {
        PamelloOutput.Write("Adding providers");
        foreach (var repositoryDescriptor in _repositoriesLoader.RepositoriesDescriptors) {
            var repository = services.GetRequiredService(repositoryDescriptor.RepositoryType) as IPamelloRepository;
            if (repository is null) continue;

            PamelloOutput.Write($"| {repositoryDescriptor.Attribute.EntityType.Name} \"{repositoryDescriptor.Attribute.ProviderName}\"");
            
            Providers.Add(new PamelloQueryProviderDescriptor(
                repositoryDescriptor.Attribute.ProviderName,
                repositoryDescriptor.Attribute.EntityType,
                repository
            ));
        }
    }

    private PamelloQueryProviderDescriptor? GetProviderFor<TPamelloEntity>() {
        return Providers.FirstOrDefault(p => typeof(TPamelloEntity).IsAssignableTo(p.EntityType));
    }
    private PamelloQueryProviderDescriptor? GetRepositoryFor<TPamelloEntity>() {
        return Providers.FirstOrDefault(p => typeof(TPamelloEntity).IsAssignableTo(p.EntityType));
    }

    public Task<IEnumerable<TPamelloEntity>> GetAsync<TPamelloEntity>(string query) {
        throw new NotImplementedException();
    }
    public TPamelloEntity? GetSingleById<TPamelloEntity>(int id)
        where TPamelloEntity : class, IPamelloBasicEntity
    {
        var provider = GetProviderFor<TPamelloEntity>();
        if (provider is null) return null;
        
        return provider.Repository.Get<TPamelloEntity>(id);
    }
    public IEnumerable<TPamelloEntity> GetByIds<TPamelloEntity>(params int[] ids) {
        throw new NotImplementedException();
    }
}
