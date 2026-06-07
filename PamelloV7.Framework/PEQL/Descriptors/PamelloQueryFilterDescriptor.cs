using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Core.PEQL.Attributes;
using PamelloV7.Framework.Core.PEQL.Blocks;
using PamelloV7.Framework.Core.PEQL.Operators;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.PEQL.Descriptors;

public class PamelloQueryFilterDescriptor(
    IPamelloQueryFilterAttribute attribute,
    Type type
)
{
    public readonly IPamelloQueryFilterAttribute Attribute = attribute;
    public readonly Type Type = type;
}
