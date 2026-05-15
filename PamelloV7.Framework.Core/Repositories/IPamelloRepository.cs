using PamelloV7.Framework.Core.Exceptions;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.Repositories;

public interface IPamelloRepository
{
    public void DeleteAll();
    public IEnumerable<IPamelloBasicEntity> GetAll();
    public IPamelloBasicEntity? Get(int id);
}

public interface IPamelloRepository<TEntity> : IPamelloRepository
    where TEntity : class, IPamelloBasicEntity
{
    IEnumerable<IPamelloBasicEntity> IPamelloRepository.GetAll() => GetAll();
    public new IEnumerable<TEntity> GetAll();
    
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
        
        public IEnumerable<TEntity> GetAll<TEntity>()
            where TEntity : class, IPamelloBasicEntity
            => repository.GetAll().OfType<TEntity>();
    }
}