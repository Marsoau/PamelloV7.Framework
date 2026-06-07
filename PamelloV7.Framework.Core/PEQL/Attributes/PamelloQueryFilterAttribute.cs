using PamelloV7.Framework.Core.PEQL.Filters;
using PamelloV7.Framework.Core.PEQL.Operators;
using PamelloV7.Framework.Shared.Attributes;

namespace PamelloV7.Framework.Core.PEQL.Attributes;

[AttributeUsage(AttributeTargets.Class)]

[AutoInherit(typeof(PamelloQueryFilter), [])]

public class PamelloQueryFilterAttribute : Attribute
{
    public string[] Names { get; set; }
    
    public string? Description { get; }
    
    public PamelloQueryFilterAttribute(string name, string? description = null) : this([name], description) { }
    public PamelloQueryFilterAttribute(string[] names, string? description = null) {
        Names = names;
        Description = description;
    }
    
    public override string ToString() => string.Join(", ", Names);
}
