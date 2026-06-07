using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Core.Actions;
using PamelloV7.Framework.Core.PEQL.Actions;
using PamelloV7.Framework.Core.PEQL.Blocks;
using PamelloV7.Framework.Shared.Entities.Base;
using PamelloV7.Framework.Shared.Variants.Attributes;

namespace PamelloV7.Framework.Core.PEQL.Filters;

public interface IPamelloQueryFilter<out TPamelloEntity> : IPamelloQueryActions<TPamelloEntity>
    where TPamelloEntity : class, IPamelloBasicEntity
{
    public IAsyncEnumerable<TPamelloEntity> ExecuteByArgsAsync(PamelloQueryBlock? arg);
}

public abstract partial class PamelloQueryFilter<TPamelloEntity> : PamelloQueryActions<TPamelloEntity>, IPamelloQueryFilter<TPamelloEntity>
    where TPamelloEntity : class, IPamelloBasicEntity
{
    public async IAsyncEnumerable<TPamelloEntity> ExecuteByArgsAsync(PamelloQueryBlock? arg) {
        var executeMethod = GetType().GetMethod("Execute")!;
        
        var peql = Services.GetRequiredService<IPamelloEntityQueryService>();
        
        var arguments = await PamelloStaticActions.EnumerateArgumentsForParameters(
            arg?.Text ?? "",
            executeMethod.GetParameters(),
            peql
        ).ToArrayAsync();

        await foreach (var entity in (IAsyncEnumerable<TPamelloEntity>)executeMethod.Invoke(this, arguments)!) {
            yield return entity;
        };
    }
}
