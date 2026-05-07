using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.Repositories;

public interface IPamelloLazyDatabaseRepository : IPamelloRepository
{
    public string CollectionName { get; }
}

public interface IPamelloLazyDatabaseRepository<TEntity> : IPamelloLazyDatabaseRepository, IPamelloRepository<TEntity>
    where TEntity : class, IPamelloBasicEntity;
