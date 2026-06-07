using PamelloV7.Framework.Core.PEQL.Attributes;
using PamelloV7.Framework.Core.PEQL.Filters;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.PEQL.Filters;

[PamelloQueryFilter("shuffle")]
public partial class PamelloQueryShuffleFilter
{
    public IAsyncEnumerable<IPamelloBasicEntity> Execute() {
        return GetEntities().OrderBy(_ => Random.Shared.Next());
    }
}
