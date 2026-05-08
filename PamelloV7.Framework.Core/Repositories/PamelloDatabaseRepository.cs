using PamelloV7.Framework.Core.Entities;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.Repositories;

public abstract class PamelloDatabaseRepository<TPamelloEntity, TEntityData>
    : PamelloLazyDatabaseRepository<TPamelloEntity, TEntityData>, IPamelloDatabaseRepository<TPamelloEntity>
    where TPamelloEntity : class, IPamelloBasicEntity
    where TEntityData : DatabaseEntityData
{
    public PamelloDatabaseRepository(IServiceProvider services) : base(services) { }
    
    public void LoadAll() {
        
    }
    public void InitAll() {
        
    }
}
