using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Core.PEQL.Actions;
using PamelloV7.Framework.Core.PEQL.Blocks;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.PEQL.Operators;

public abstract class PamelloQueryOperator : PamelloQueryActions
{
    public abstract IAsyncEnumerable<IPamelloBasicEntity> Execute(PamelloQueryBlock? arg);
}

