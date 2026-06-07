using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Core.PEQL;
using PamelloV7.Framework.Core.PEQL.Attributes;
using PamelloV7.Framework.Core.PEQL.Blocks;
using PamelloV7.Framework.Core.PEQL.Filters;
using PamelloV7.Framework.PEQL.Loaders;
using PamelloV7.Framework.Shared.Entities.Base;
using PamelloV7.Framework.Shared.Exceptions;

namespace PamelloV7.Framework.PEQL.Operators;

[PamelloQueryOperator('#', "Filter", "Filter entities by a specified filter")]
public partial class PamelloQueryFilterOperator
{
    public override IAsyncEnumerable<IPamelloBasicEntity> Execute(PamelloQueryBlock? arg) {
        var loader = Services.GetRequiredService<PamelloEntityQueryLanguageLoader>();

        var peqlFilterDescriptor = loader.FiltersDescriptors.FirstOrDefault(o =>
            o.Attribute.Names.Contains(arg?.Text)
        );
        if (peqlFilterDescriptor is null) throw new PamelloException($"Filter \"{arg?.Text}\" not found");
        
        var filter = (PamelloQueryFilter)Services.GetRequiredService(peqlFilterDescriptor.Type);
        
        filter.InitializeQueryActions(Services, GetEntities);
        
        return filter.ExecuteByArgsAsync(arg);
    }
}
