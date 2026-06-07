using PamelloV7.Framework.Core.PEQL.Operators;
using PamelloV7.Framework.Shared.Attributes;

namespace PamelloV7.Framework.Core.PEQL.Attributes;

[AttributeUsage(AttributeTargets.Class)]

public class PamelloQueryOperatorAttribute : Attribute
{
    public char Operator { get; }
    public string Name { get; }
    public string? Description { get; }
    
    public PamelloQueryOperatorAttribute(char c, string name, string? description = null) {
        Operator = c;
        Name = name;
        Description = description;
    }
}
