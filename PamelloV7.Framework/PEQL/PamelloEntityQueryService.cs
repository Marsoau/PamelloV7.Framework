using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Core.Logging;
using PamelloV7.Framework.Core.PEQL;
using PamelloV7.Framework.Core.Repositories;
using PamelloV7.Framework.Core.Scope;
using PamelloV7.Framework.Repositories.Loaders;
using PamelloV7.Framework.Shared.Entities.Base;
using PamelloV7.Framework.Shared.Exceptions;
using PamelloV7.Framework.Shared.Variants.Attributes;

namespace PamelloV7.Framework.PEQL;

public record PamelloQueryProviderDescriptor(
    string Name,
    Type EntityType,
    IPamelloRepository Repository
);

public partial class PamelloEntityQueryService : IPamelloEntityQueryService
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
    
    private static Type GetEntityTypeGeneric<TEntityType>() => typeof(TEntityType);
    private PamelloQueryProviderDescriptor? GetProviderForEntityType(
        [Variant(nameof(GetEntityTypeGeneric))]
        Type entityType
    ) {
        return Providers.FirstOrDefault(p => entityType.IsAssignableTo(p.EntityType));
    }

    public TPamelloEntity? GetSingleById<TPamelloEntity>(int id)
        where TPamelloEntity : class, IPamelloBasicEntity
    {
        var provider = GetProviderForEntityType<TPamelloEntity>();
        if (provider is null) return null;
        
        return provider.Repository.Get<TPamelloEntity>(id);
    }
    public IAsyncEnumerable<TPamelloEntity> GetByIds<TPamelloEntity>(params int[] ids) {
        throw new NotImplementedException();
    }
    
    public async IAsyncEnumerable<TPamelloEntity> GetAsync<TPamelloEntity>(string query) {
        if (PamelloAppScope.User is null) throw new PamelloException("User is required to execute PEQL queries");
        
        //songs$random*3
        
        //songs$all((1,2))#{Length>3:00}
        //|songs|$|all|((1,2))|#|{Length>3:00}|
        
        yield break;
    }
    
    
    //
    //
    //
}
