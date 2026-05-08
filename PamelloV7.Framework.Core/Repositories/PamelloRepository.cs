using PamelloV7.Framework.Core.Exceptions;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.Repositories;

public abstract class PamelloRepository<TPamelloEntity> : IPamelloRepository<TPamelloEntity>
    where TPamelloEntity : class, IPamelloBasicEntity
{
    protected readonly IServiceProvider Services;
    
    protected List<TPamelloEntity> Loaded = [];
    protected IEnumerable<TPamelloEntity> Available => Loaded.Where(e => e.IsAvailable());

    public PamelloRepository(IServiceProvider services) {
        Services = services;
    }

    public virtual IEnumerable<TPamelloEntity> GetAll() => Available;

    public virtual TPamelloEntity? Get(int id)
        => Available.FirstOrDefault(e => e.Id == id);

    protected virtual TPamelloEntity LoadPamelloEntity(TPamelloEntity entity) {
        if (Loaded.Any(e => e.Id == entity.Id)) throw new PamelloDatabaseException($"Entity with id {entity.Id} already loaded to {GetType().Name}");
        
        Loaded.Add(entity);
        
        return entity;
    }
}
