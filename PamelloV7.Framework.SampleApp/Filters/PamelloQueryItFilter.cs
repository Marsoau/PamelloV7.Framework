using PamelloV7.Framework.Core.PEQL.Attributes;
using PamelloV7.Framework.Core.PEQL.Filters;
using PamelloV7.Framework.SampleApp.Entities;

namespace PamelloV7.Framework.SampleApp.Filters;

[PamelloQueryFilter<Item>("it")]
public partial class PamelloQueryItFilter
{
    public IAsyncEnumerable<Item> Execute() {
        return GetEntities().Where(item => item.Id % 2 == 0);
    }
}
