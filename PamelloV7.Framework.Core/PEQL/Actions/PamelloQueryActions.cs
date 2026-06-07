using PamelloV7.Framework.Core.Actions;
using PamelloV7.Framework.Shared.Entities.Base;
using PamelloV7.Framework.Shared.Variants.Attributes;

namespace PamelloV7.Framework.Core.PEQL.Actions;

public abstract partial class PamelloQueryActions : PamelloBasicActions
{
    private Func<IAsyncEnumerable<IPamelloBasicEntity>> _getEntities = () => Enumerable.Empty<IPamelloBasicEntity>().ToAsyncEnumerable();
    
    private static Func<IAsyncEnumerable<IPamelloBasicEntity>> FuncFromEnumerable<T>(IEnumerable<T> enumerable)
        where T : class, IPamelloBasicEntity
        => enumerable.ToAsyncEnumerable;
    
    private static Func<IAsyncEnumerable<IPamelloBasicEntity>> FuncFromAsyncEnumerable<T>(IAsyncEnumerable<T> asyncEnumerable)
        where T : class, IPamelloBasicEntity
        => () => asyncEnumerable;
    
    private Func<IAsyncEnumerable<IPamelloBasicEntity>> FuncFromQuery(string query) => () => PEQL.GetAsync(query);

    public void InitializeQueryActions(
        IServiceProvider services,
        [Variant(nameof(FuncFromQuery))]
        [Variant(nameof(FuncFromEnumerable))]
        [Variant(nameof(FuncFromAsyncEnumerable))]
        Func<IAsyncEnumerable<IPamelloBasicEntity>>? getEntities = null
    ) {
        InitializeActions(services);
        
        _getEntities = getEntities ?? _getEntities;
    }
    
    public IAsyncEnumerable<IPamelloBasicEntity> GetEntities() => _getEntities();
}
