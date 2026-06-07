using PamelloV7.Framework.Core.Entities;
using PamelloV7.Framework.Core.PEQL;
using PamelloV7.Framework.Core.PEQL.Attributes;
using PamelloV7.Framework.Core.PEQL.Blocks;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.PEQL.Filters;

[PamelloQueryFilter("test")]
public partial class PamelloQueryTestFilter
{
    public PamelloQueryTestFilter(IServiceProvider services) : base(services) { }
    
    public async IAsyncEnumerable<IPamelloBasicEntity> Execute() {
        await foreach (var entity in GetEntities().Take(3))
            yield return entity;
    }
}
