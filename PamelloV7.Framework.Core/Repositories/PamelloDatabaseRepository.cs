using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Core.Data;
using PamelloV7.Framework.Core.Entities;
using PamelloV7.Framework.Core.Entities.Base;
using PamelloV7.Framework.Core.Entities.Dbo;
using PamelloV7.Framework.Core.Exceptions;
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
        _ = GetCollection().GetAll().Select(LoadDatabaseEntity).ToList();
    }

    IDatabaseCollection<PamelloBasicDbo> IPamelloDatabaseRepository.GetCollection() {
        return Database.GetCollection<PamelloBasicDbo>(CollectionName);
    }
    public IDatabaseCollection<TEntityDbo> GetCollection() {
        return Database.GetCollection<TEntityDbo>(CollectionName);
    }

    protected virtual TPamelloEntity LoadDatabaseEntity(TEntityDbo dbo) {
        var constructor = typeof(TPamelloEntity).GetConstructor([typeof(TEntityDbo)]);
        if (constructor is null) throw new PamelloDatabaseException($"Entity {typeof(TPamelloEntity).Name} must have constructor with single parameter of type {typeof(TEntityDbo).Name}");
        
        return base.LoadPamelloEntity((TPamelloEntity)constructor.Invoke([dbo]));
    }
    protected override TPamelloEntity LoadPamelloEntity(TPamelloEntity entity) {
        entity.Save();
        return base.LoadPamelloEntity(entity);
    }
}
