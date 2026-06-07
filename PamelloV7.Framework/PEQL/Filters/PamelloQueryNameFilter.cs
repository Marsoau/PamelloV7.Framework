using PamelloV7.Framework.Core.PEQL.Attributes;
using PamelloV7.Framework.Core.PEQL.Blocks;
using PamelloV7.Framework.Core.PEQL.Filters;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.PEQL.Filters;

[PamelloQueryFilter("name")]
public partial class PamelloQueryNameFilter : PamelloQueryFilter<IPamelloBasicEntity>
{
    public async IAsyncEnumerable<IPamelloBasicEntity> Execute(string nameWildcard) {
        var blocks = nameWildcard.EnumerateStringBlocks(['*'], true);

        await foreach (var entity in GetEntities()) {
            yield return entity;
        }
    }
}
