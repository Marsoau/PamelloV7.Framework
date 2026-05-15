using PamelloV7.Framework.Core.Data;
using PamelloV7.Framework.Core.Entities.Dao;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.Repositories;

public interface IPamelloDatabaseRepository : IPamelloRepository
{
    public void LoadAll();
    
    public string CollectionName { get; }
    public IDatabaseCollection<PamelloBasicDao> GetCollection();
}

public interface IPamelloDatabaseRepository<TEntity> : IPamelloDatabaseRepository, IPamelloRepository<TEntity>
    where TEntity : class, IPamelloBasicEntity;
