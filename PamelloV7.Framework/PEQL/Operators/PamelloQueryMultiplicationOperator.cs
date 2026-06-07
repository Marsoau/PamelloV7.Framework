using PamelloV7.Framework.Core.PEQL;
using PamelloV7.Framework.Core.PEQL.Attributes;
using PamelloV7.Framework.Core.PEQL.Blocks;
using PamelloV7.Framework.Core.PEQL.Operators;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.PEQL.Operators;

[PamelloQueryOperator('*', "Multiplication", "Repeats query a specified number of times")]
public partial class PamelloQueryMultiplicationOperator : PamelloQueryOperator<IPamelloBasicEntity>
{
    public override async IAsyncEnumerable<IPamelloBasicEntity> Execute(PamelloQueryBlock? arg) {
        var repeatCount = int.Parse(arg?.Text ?? "nonumber");

        for (var i = 0; i < repeatCount; i++)
            await foreach (var entity in GetEntities())
                yield return entity;
    }
}
