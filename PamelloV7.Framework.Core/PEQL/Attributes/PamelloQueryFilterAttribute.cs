using PamelloV7.Framework.Core.PEQL.Filters;
using PamelloV7.Framework.Core.PEQL.Operators;
using PamelloV7.Framework.Shared.Attributes;

namespace PamelloV7.Framework.Core.PEQL.Attributes;

[AttributeUsage(AttributeTargets.Class)]

[AutoInherit(typeof(PamelloQueryFilter), [])]

public class PamelloQueryFilterAttribute : Attribute
{
    public string Name { get; }
    public string? Description { get; }
    
    public PamelloQueryFilterAttribute(string name, string? description = null) {
        Name = name;
        Description = description;
    }
}
