using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Core.PEQL.Attributes;
using PamelloV7.Framework.Core.PEQL.Blocks;
using PamelloV7.Framework.Core.PEQL.Operators;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.PEQL.Descriptors;

public class PamelloQueryOperatorDescriptor(
    PamelloQueryOperatorAttribute attribute,
    Type operatorClassType
)
{
    public readonly PamelloQueryOperatorAttribute Attribute = attribute;
    public readonly Type OperatorClassType = operatorClassType;

    public IAsyncEnumerable<IPamelloBasicEntity> Execute(string query, PamelloQueryBlock arg, IServiceProvider services) {
        var instance = (PamelloQueryOperator)services.GetRequiredService(OperatorClassType);
        return instance.Execute(query, arg);
    }
}
