using PamelloV7.Framework.Core.Repositories.Attributes;
using PamelloV7.Framework.Shared.Attributes;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.Repositories.Attributes;

[AttributeUsage(AttributeTargets.Class)]

public class PamelloRepositoryBaseTypeAttribute : Attribute
{
    public PamelloRepositoryBaseTypeAttribute(Type baseType) { }
}

