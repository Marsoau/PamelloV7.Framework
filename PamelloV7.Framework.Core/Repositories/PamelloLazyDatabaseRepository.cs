using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Core.Data;
using PamelloV7.Framework.Core.Entities;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.Repositories;

public abstract class PamelloLazyDatabaseRepository<TPamelloEntity, TEntityData>
    : PamelloRepository<TPamelloEntity>, IPamelloLazyDatabaseRepository<TPamelloEntity>
    where TPamelloEntity : class, IPamelloBasicEntity
    where TEntityData : DatabaseEntityData
{
    protected readonly IDatabaseAccessService Database;
    
    public abstract string CollectionName { get; }

    public PamelloLazyDatabaseRepository(IServiceProvider services) : base(services) {
        Database = services.GetRequiredService<IDatabaseAccessService>();
    }
    
    public IDatabaseCollection<TEntityData> GetCollection() {
        return Database.GetCollection<TEntityData>(CollectionName);
    }

    protected virtual TPamelloEntity LoadDatabaseEntity(TEntityData entity) {
        return null;
    }
}
