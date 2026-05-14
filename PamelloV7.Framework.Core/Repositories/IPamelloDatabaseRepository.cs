using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.Repositories;

public interface IPamelloLazyDatabaseRepository : IPamelloRepository
{
    public void LoadAll();
    public string CollectionName { get; }
}

public interface IPamelloDatabaseRepository<TEntity> : IPamelloLazyDatabaseRepository, IPamelloRepository<TEntity>
    where TEntity : class, IPamelloBasicEntity;
