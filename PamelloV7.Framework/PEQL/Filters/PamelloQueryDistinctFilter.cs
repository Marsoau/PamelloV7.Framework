using PamelloV7.Framework.Core.PEQL.Attributes;
using PamelloV7.Framework.Core.PEQL.Filters;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.PEQL.Filters;

[PamelloQueryFilter(["distinct", "unique"])]
public partial class PamelloQueryDistinctFilter : PamelloQueryFilter<IPamelloBasicEntity>
{
    public IAsyncEnumerable<IPamelloBasicEntity> Execute() {
        return GetEntities().Distinct();
    }
}
