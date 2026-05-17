using PamelloV7.Framework.Core.Exceptions;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.Repositories;

public interface IPamelloRepository
{
    public IPamelloBasicEntity? Get(int id);
    public IEnumerable<IPamelloBasicEntity> GetAll();
    public void DeleteAll();
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
    public static IPamelloBasicEntity GetRequired(this IPamelloRepository repository, int id)
        => repository.Get(id) ?? throw new PamelloDatabaseException($"Entity with id {id} not found");
    public static IPamelloBasicEntity GetRequired(this IPamelloRepository repository, int id, Type type)
        => repository.Get(id, type) ?? throw new PamelloDatabaseException($"Entity with id {id} not found");
    public static TEntity GetRequired<TEntity>(this IPamelloRepository<TEntity> repository, int id)
        where TEntity : class, IPamelloBasicEntity
        => repository.Get(id, typeof(TEntity)) as TEntity ?? throw new PamelloDatabaseException($"Entity with id {id} not found");

    public static IPamelloBasicEntity? Get(this IPamelloRepository repository, int id, Type type)
        => repository.Get(id) is { } entity && entity.GetType().IsAssignableTo(type) ? entity : null;
    public static TEntity? Get<TEntity>(this IPamelloRepository repository, int id) where TEntity : class, IPamelloBasicEntity
        => repository.Get(id) as TEntity;

    public static IEnumerable<TEntity> GetAll<TEntity>(this IPamelloRepository repository)
        where TEntity : class, IPamelloBasicEntity
        => repository.GetAll().OfType<TEntity>();
}