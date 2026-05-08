using PamelloV7.Framework.Core.Exceptions;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.Repositories;

public interface IPamelloRepository
{
    public IPamelloBasicEntity? Get(int id);
}

public interface IPamelloRepository<TEntity> : IPamelloRepository
    where TEntity : class, IPamelloBasicEntity
{
    IPamelloBasicEntity? IPamelloRepository.Get(int id) => Get(id);
    public new TEntity? Get(int id);
}

public static class PamelloRepositoryExtensions
{
    extension(IPamelloRepository repository)
    {
        public IPamelloBasicEntity GetRequired(int id)
            => repository.Get(id) ?? throw new PamelloDatabaseException($"Entity with id {id} not found");
        public IPamelloBasicEntity GetRequired(int id, Type type)
            => repository.Get(id, type) ?? throw new PamelloDatabaseException($"Entity with id {id} not found");
        public TEntity GetRequired<TEntity>(int id)
            where TEntity : class, IPamelloBasicEntity
            => repository.Get<TEntity>(id) ?? throw new PamelloDatabaseException($"Entity with id {id} not found");
        
        public IPamelloBasicEntity? Get(int id, Type type)
            => repository.Get(id) is { } entity && entity.GetType() == type ? entity : null;
        public TEntity? Get<TEntity>(int id) where TEntity : class, IPamelloBasicEntity
            => repository.Get(id) as TEntity;
    }
}