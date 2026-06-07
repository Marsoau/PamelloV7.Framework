using PamelloV7.Framework.Core.Actions;
using PamelloV7.Framework.Shared.Entities.Base;
using PamelloV7.Framework.Shared.Variants.Attributes;

namespace PamelloV7.Framework.Core.PEQL.Actions;

public interface IPamelloQueryActions<out TPamelloEntity>
{
    void InitializeQueryActions(IServiceProvider services, string query);
    void InitializeQueryActions(IServiceProvider services, IEnumerable<IPamelloBasicEntity> entities);
    void InitializeQueryActions(IServiceProvider services, IAsyncEnumerable<IPamelloBasicEntity> entities);
    void InitializeQueryActions(IServiceProvider services, Func<IAsyncEnumerable<IPamelloBasicEntity>> entities);
    
    IAsyncEnumerable<TPamelloEntity> GetEntities();
}

public abstract partial class PamelloQueryActions<TPamelloEntity> : PamelloBasicActions, IPamelloQueryActions<TPamelloEntity>
    where TPamelloEntity : class, IPamelloBasicEntity
{
    private Func<IAsyncEnumerable<TPamelloEntity>> _getEntities = () => Enumerable.Empty<TPamelloEntity>().ToAsyncEnumerable();
    
    private static Func<IAsyncEnumerable<TPamelloEntity>> FuncFromAsyncBasicFunc(Func<IAsyncEnumerable<IPamelloBasicEntity>> getBasicEntitiesAsync)
        => () => getBasicEntitiesAsync().OfType<TPamelloEntity>();
    private static Func<IAsyncEnumerable<TPamelloEntity>> FuncFromBasicFunc(Func<IEnumerable<IPamelloBasicEntity>> getBasicEntities)
        => () => getBasicEntities().OfType<TPamelloEntity>().ToAsyncEnumerable();
    
    private static Func<IAsyncEnumerable<TPamelloEntity>> FuncFromEnumerable(IEnumerable<IPamelloBasicEntity> enumerable)
        => () => enumerable.OfType<TPamelloEntity>().ToAsyncEnumerable();
    
    private static Func<IAsyncEnumerable<TPamelloEntity>> FuncFromAsyncEnumerable(IAsyncEnumerable<IPamelloBasicEntity> asyncEnumerable)
        => asyncEnumerable.OfType<TPamelloEntity>;
    
    private Func<IAsyncEnumerable<TPamelloEntity>> FuncFromQuery(string query) => () => PEQL.GetAsync<TPamelloEntity>(query);

    public void InitializeQueryActions(
        IServiceProvider services,
        [Variant(nameof(FuncFromQuery))]
        [Variant(nameof(FuncFromEnumerable))]
        [Variant(nameof(FuncFromAsyncEnumerable))]
        [Variant(nameof(FuncFromAsyncBasicFunc))]
        [Variant(nameof(FuncFromBasicFunc))]
        Func<IAsyncEnumerable<TPamelloEntity>>? getEntities = null
    ) {
        InitializeActions(services);
        
        _getEntities = getEntities ?? _getEntities;
    }

    public IAsyncEnumerable<TPamelloEntity> GetEntities() => _getEntities();
}
