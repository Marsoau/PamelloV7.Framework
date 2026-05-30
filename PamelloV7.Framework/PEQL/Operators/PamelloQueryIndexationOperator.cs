using PamelloV7.Framework.Core.PEQL;
using PamelloV7.Framework.Core.PEQL.Attributes;
using PamelloV7.Framework.Core.PEQL.Blocks;
using PamelloV7.Framework.Core.PEQL.Range;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.PEQL.Operators;

[PamelloQueryOperator(':', "Indexation", "Get entity at a specified positions range")]
public partial class PamelloQueryIndexationOperator
{
    public PamelloQueryIndexationOperator(IServiceProvider services) : base(services) { }
    
    public override async IAsyncEnumerable<IPamelloBasicEntity> Execute(string query, PamelloQueryBlock? arg) {
        var result = await PEQL.GetAsync(query).ToListAsync();

        var range = result.GetRange(result.Count, PamelloQueryRange.Parse(arg?.Text ?? ""));
        
        foreach (var entity in range)
            yield return entity;
    }
}
