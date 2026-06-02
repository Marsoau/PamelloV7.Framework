using PamelloV7.Framework.Core.PEQL;
using PamelloV7.Framework.Core.PEQL.Attributes;
using PamelloV7.Framework.Core.PEQL.Blocks;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.PEQL.Operators;

[PamelloQueryOperator('*', "Multiplication", "Repeats query a specified number of times")]
public partial class PamelloQueryMultiplicationOperator
{
    public PamelloQueryMultiplicationOperator(IServiceProvider services) : base(services) { }

    public override async IAsyncEnumerable<IPamelloBasicEntity> Execute(string query, PamelloQueryBlock? arg) {
        var repeatCount = int.Parse(arg?.Text ?? "nonumber");

        for (var i = 0; i < repeatCount; i++)
            await foreach (var entity in PEQL.GetAsync(query))
                yield return entity;
    }
}
