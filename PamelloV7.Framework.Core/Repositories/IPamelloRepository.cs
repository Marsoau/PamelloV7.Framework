using PamelloV7.Framework.Core.Exceptions;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.Repositories;

public interface IPamelloRepository
{
    public IPamelloBasicEntity? Get(int id) => Get<IPamelloBasicEntity>(id);
    
    public IPamelloBasicEntity? Get(int id, Type type) {
        var getByIdMethods = typeof(IPamelloRepository).GetMethods()
            .FirstOrDefault(method => method.Name == nameof(Get) && method.GetGenericArguments().Length == 1);

        return getByIdMethods?.MakeGenericMethod(type).Invoke(this, [id]) as IPamelloBasicEntity;
    }
    
    public TType? Get<TType>(int id)
        where TType : class, IPamelloBasicEntity;
}

public interface IPamelloRepository<TEntity> : IPamelloRepository
    where TEntity : class, IPamelloBasicEntity
{
    IPamelloBasicEntity? IPamelloRepository.Get(int id) => Get(id);
    public new TEntity? Get(int id) => Get<TEntity>(id);
    
    IPamelloBasicEntity? IPamelloRepository.Get(int id, Type type) => Get(id, type);

    public new TEntity? Get(int id, Type type) {
        var genericGet = typeof(IPamelloRepository).GetMethods()
            .FirstOrDefault(method => method.Name == nameof(Get) && method.GetGenericArguments().Length == 1);

        var genericType = genericGet?.GetGenericArguments().FirstOrDefault();
        if (!genericType?.IsAssignableTo(type) ?? true) return null;
        
        return genericGet?.MakeGenericMethod(type).Invoke(this, [id]) as TEntity;
    }
}
