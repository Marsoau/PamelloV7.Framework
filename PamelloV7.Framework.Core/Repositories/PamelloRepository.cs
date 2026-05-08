using PamelloV7.Framework.Core.Exceptions;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.Repositories;

public abstract class PamelloRepository<TEntity> : IPamelloRepository<TEntity>
    where TEntity : class, IPamelloBasicEntity
{
    protected readonly IServiceProvider Services;
    
    protected List<TEntity> Loaded = [];
    protected IEnumerable<TEntity> Available => Loaded.Where(e => e.IsAvailable());

    public PamelloRepository(IServiceProvider services) {
        Services = services;
    }

    public virtual TEntity? Get(int id)
        => Available.FirstOrDefault(e => e.Id == id);

    protected virtual TEntity Load(TEntity entity) {
        if (Loaded.Any(e => e.Id == entity.Id)) throw new PamelloDatabaseException($"Entity with id {entity.Id} already loaded to {GetType().Name}");
        
        Loaded.Add(entity);
        
        return entity;
    }
}
