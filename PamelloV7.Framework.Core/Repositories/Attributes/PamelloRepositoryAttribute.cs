using PamelloV7.Framework.Shared.Attributes;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.Repositories.Attributes;

[AttributeUsage(AttributeTargets.Class)]

[PamelloRepositoryBaseType(typeof(PamelloRepository<>))]

public class PamelloRepositoryAttribute<TPamelloEntity> : Attribute
    where TPamelloEntity : class, IPamelloBasicEntity
{
    public PamelloRepositoryAttribute(string providerName) { }
}
