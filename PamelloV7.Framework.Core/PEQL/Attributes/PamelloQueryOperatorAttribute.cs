using PamelloV7.Framework.Core.PEQL.Operators;
using PamelloV7.Framework.Shared.Attributes;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.PEQL.Attributes;

public interface IPamelloQueryOperatorAttribute
{
    char Operator { get; }
    string Name { get; }
    string? Description { get; }
    public Type EntitiesType { get; }
}

[AttributeUsage(AttributeTargets.Class)]

[AutoInherit(typeof(PamelloQueryOperator<IPamelloBasicEntity>), [])]

public class PamelloQueryOperatorAttribute : PamelloQueryOperatorAttribute<IPamelloBasicEntity>
{
    public PamelloQueryOperatorAttribute(char c, string name, string? description = null) : base(c, name, description) { }
}

[AttributeUsage(AttributeTargets.Class)]

[AutoInherit(typeof(PamelloQueryOperator<>), [])]

public class PamelloQueryOperatorAttribute<TPamelloEntity> : Attribute, IPamelloQueryOperatorAttribute
    where TPamelloEntity : class, IPamelloBasicEntity
{
    public char Operator { get; }
    public string Name { get; }
    public string? Description { get; }
    public Type EntitiesType => typeof(TPamelloEntity);

    public PamelloQueryOperatorAttribute(char c, string name, string? description = null) {
        Operator = c;
        Name = name;
        Description = description;
    }
}
