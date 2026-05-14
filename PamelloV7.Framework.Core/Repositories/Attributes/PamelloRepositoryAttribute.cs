using PamelloV7.Framework.Shared.Attributes;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.Repositories.Attributes;

public interface IPamelloRepositoryAttribute
{
    public string ProviderName { get; }
    public Type EntityType { get; }
}

[AttributeUsage(AttributeTargets.Class)]

[PamelloRepositoryBaseType(typeof(PamelloRepository<>))]

public class PamelloRepositoryAttribute<TPamelloEntity> : Attribute, IPamelloRepositoryAttribute
    where TPamelloEntity : class, IPamelloBasicEntity
{
    public string ProviderName { get; }
    public Type EntityType => typeof(TPamelloEntity);

    public PamelloRepositoryAttribute(string providerName) {
        ProviderName = providerName;
    }
}
