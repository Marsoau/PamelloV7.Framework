using PamelloV7.Framework.Core.Exceptions;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.Repositories;

public class PamelloRepository<TEntity> : IPamelloRepository<TEntity>
    where TEntity : class, IPamelloBasicEntity
{
    protected readonly IServiceProvider Services;
    
    protected List<TEntity> Loaded = [];

    public PamelloRepository(IServiceProvider services) {
        Services = services;
    }
    
    public virtual TType? Get<TType>(int id) where TType : class, IPamelloBasicEntity {
        if (Loaded.FirstOrDefault(e => e.Id == id) is TType entity && entity.IsAvailable())
            return entity;
        
        return null;
    }
}
