using PamelloV7.Framework.Core.Entities;
using PamelloV7.Framework.Core.PEQL.Filters;
using PamelloV7.Framework.Core.PEQL.Operators;
using PamelloV7.Framework.Shared.Attributes;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.PEQL.Attributes;

public interface IPamelloQueryFilterAttribute
{
    public string[] Names { get; }
    public string? Description { get; }
    public Type EntitiesType { get; }
}


[AttributeUsage(AttributeTargets.Class)]

[AutoInherit(typeof(PamelloQueryFilter<IPamelloBasicEntity>), [])]

public class PamelloQueryFilterAttribute : PamelloQueryFilterAttribute<IPamelloBasicEntity>
{
    public PamelloQueryFilterAttribute(string name, string? description = null) : base(name, description) { }
    public PamelloQueryFilterAttribute(string[] names, string? description = null) : base(names, description) { }
}

[AttributeUsage(AttributeTargets.Class)]

[AutoInherit(typeof(PamelloQueryFilter<>), [])]

public class PamelloQueryFilterAttribute<TPamelloEntity> : Attribute, IPamelloQueryFilterAttribute
    where TPamelloEntity : class, IPamelloBasicEntity
{
    public string[] Names { get; set; }
    public string? Description { get; }
    public Type EntitiesType => typeof(TPamelloEntity);
    
    public PamelloQueryFilterAttribute(string name, string? description = null) : this([name], description) { }
    public PamelloQueryFilterAttribute(string[] names, string? description = null) {
        Names = names;
        Description = description;
    }
    
    public override string ToString() => string.Join(", ", Names);
}
