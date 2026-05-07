using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.Repositories;

public interface IPamelloDatabaseRepository : IPamelloLazyDatabaseRepository
{
    public void LoadAll();
    public void InitAll();
}

public interface IPamelloDatabaseRepository<TEntity> : IPamelloDatabaseRepository, IPamelloLazyDatabaseRepository<TEntity>
    where TEntity : class, IPamelloBasicEntity;
