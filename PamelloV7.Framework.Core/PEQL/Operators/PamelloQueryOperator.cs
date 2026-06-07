using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Core.PEQL.Actions;
using PamelloV7.Framework.Core.PEQL.Blocks;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.PEQL.Operators;

public interface IPamelloQueryOperator<out TPamelloEntity> : IPamelloQueryActions<TPamelloEntity>
{
    IAsyncEnumerable<TPamelloEntity> Execute(PamelloQueryBlock? arg);
}

public abstract class PamelloQueryOperator<TPamelloEntity> : PamelloQueryActions<TPamelloEntity>, IPamelloQueryOperator<TPamelloEntity>
    where TPamelloEntity : class, IPamelloBasicEntity
{
    public abstract IAsyncEnumerable<TPamelloEntity> Execute(PamelloQueryBlock? arg);
}

