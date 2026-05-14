using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Core.Data;
using PamelloV7.Framework.Core.Entities;
using PamelloV7.Framework.Core.Entities.Base;
using PamelloV7.Framework.Core.Entities.Dbo;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.Repositories;

public abstract class PamelloDatabaseRepository<TPamelloEntity, TEntityDbo>
    : PamelloRepository<TPamelloEntity>, IPamelloDatabaseRepository<TPamelloEntity>
    where TPamelloEntity : class, IPamelloDatabaseBasicEntity
    where TEntityDbo : PamelloBasicDbo
{
    protected readonly IDatabaseAccessService Database;
    
    public abstract string CollectionName { get; }

    public PamelloDatabaseRepository(IServiceProvider services) : base(services) {
        Database = services.GetRequiredService<IDatabaseAccessService>();
    }

    public virtual void LoadAll() {
        
    }

    IDatabaseCollection<PamelloBasicDbo> IPamelloDatabaseRepository.GetCollection() {
        return Database.GetCollection<PamelloBasicDbo>(CollectionName);
    }
    public IDatabaseCollection<TEntityDbo> GetCollection() {
        return Database.GetCollection<TEntityDbo>(CollectionName);
    }

    protected override TPamelloEntity LoadPamelloEntity(TPamelloEntity entity) {
        entity.Save();
        
        return base.LoadPamelloEntity(entity);
    }
}
