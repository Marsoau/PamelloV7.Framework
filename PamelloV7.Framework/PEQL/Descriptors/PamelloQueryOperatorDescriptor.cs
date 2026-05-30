using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Core.PEQL.Attributes;
using PamelloV7.Framework.Core.PEQL.Blocks;
using PamelloV7.Framework.Core.PEQL.Operators;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.PEQL.Descriptors;

public class PamelloQueryOperatorDescriptor(
    PamelloQueryOperatorAttribute attribute,
    Type type
)
{
    public readonly PamelloQueryOperatorAttribute Attribute = attribute;
    public readonly Type Type = type;

    public IAsyncEnumerable<IPamelloBasicEntity> Execute(string query, PamelloQueryBlock arg, IServiceProvider services) {
        var instance = (PamelloQueryOperator)services.GetRequiredService(Type);
        return instance.Execute(query, arg);
    }
}
