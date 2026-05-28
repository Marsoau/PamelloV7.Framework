using PamelloV7.Framework.Shared.Entities.Base;
using PamelloV7.Framework.Shared.Exceptions;

namespace PamelloV7.Framework.Shared.Entities.Containers;

public static class SafeExtensions
{
    extension(ISafe safe)
    {
        public IPamelloBasicEntity RequiredEntity => safe.Entity
            ?? throw new PamelloException($"Entity with id {safe.Id} is no longer available");
        
    }

    public static Safe<TPamelloEntity> ToSafe<TPamelloEntity>(this IPamelloBasicEntity entity)
        where TPamelloEntity : class, IPamelloBasicEntity
        => new(entity.Id);
    public static Safe<TPamelloEntity> ToSafe<TPamelloEntity>(this int entityId)
        where TPamelloEntity : class, IPamelloBasicEntity
        => new(entityId);
    
    public static SafeCollection<TPamelloEntity> ToSafeCollection<TPamelloEntity>(this IEnumerable<TPamelloEntity> entities)
        where TPamelloEntity : class, IPamelloBasicEntity
        => new(entities);
    public static SafeCollection<TPamelloEntity> ToSafeCollection<TPamelloEntity>(this IEnumerable<int> entities)
        where TPamelloEntity : class, IPamelloBasicEntity
        => new(entities);
    
    public static async ValueTask<SafeCollection<TPamelloEntity>> ToSafeCollectionAsync<TPamelloEntity>(this IAsyncEnumerable<TPamelloEntity> entities)
        where TPamelloEntity : class, IPamelloBasicEntity
        => new(await entities.ToListAsync());
    public static async ValueTask<SafeCollection<TPamelloEntity>> ToSafeCollectionAsync<TPamelloEntity>(this IAsyncEnumerable<int> entitiesIds)
        where TPamelloEntity : class, IPamelloBasicEntity
        => new(await entitiesIds.ToListAsync());
}
