using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Core.Data;
using PamelloV7.Framework.Core.Entities;
using PamelloV7.Framework.Core.Entities.Base;
using PamelloV7.Framework.Core.Entities.Dao;
using PamelloV7.Framework.Core.Exceptions;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.Repositories;

public abstract class PamelloDatabaseRepository<TPamelloEntity, TEntityDao>
    : PamelloRepository<TPamelloEntity>, IPamelloDatabaseRepository<TPamelloEntity>
    where TPamelloEntity : class, IPamelloDatabaseBasicEntity
    where TEntityDao : PamelloBasicDao
{
    protected readonly IDatabaseAccessService Database;
    
    public abstract string CollectionName { get; }

    public PamelloDatabaseRepository(IServiceProvider services) : base(services) {
        Database = services.GetRequiredService<IDatabaseAccessService>();
    }

    public virtual void LoadAll() {
        _ = GetCollection().GetAll().Select(LoadDatabaseEntity).ToList();
    }

    IDatabaseCollection<PamelloBasicDao> IPamelloDatabaseRepository.GetCollection() {
        return Database.GetCollection<PamelloBasicDao>(CollectionName);
    }
    public IDatabaseCollection<TEntityDao> GetCollection() {
        return Database.GetCollection<TEntityDao>(CollectionName);
    }

    protected virtual TPamelloEntity LoadDatabaseEntity(TEntityDao dao) {
        var constructor = typeof(TPamelloEntity).GetConstructor([typeof(TEntityDao)]);
        if (constructor is null) throw new PamelloDatabaseException($"Entity {typeof(TPamelloEntity).Name} must have constructor with single parameter of type {typeof(TEntityDao).Name}");
        
        return base.LoadPamelloEntity((TPamelloEntity)constructor.Invoke([dao]));
    }
    
    protected override TLoadedPamelloEntity LoadPamelloEntity<TLoadedPamelloEntity>(TLoadedPamelloEntity entity) {
        entity.Save();
        return base.LoadPamelloEntity(entity);
    }
}
