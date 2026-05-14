using PamelloV7.Framework.Shared.Attributes;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.Repositories.Attributes;

[AttributeUsage(AttributeTargets.Class)]

[PamelloRepositoryBaseType(typeof(PamelloDatabaseRepository<,>))]

public class PamelloDatabaseRepositoryAttribute<TPamelloEntity> : PamelloRepositoryAttribute<TPamelloEntity>
    where TPamelloEntity : class, IPamelloBasicEntity
{
    public PamelloDatabaseRepositoryAttribute(string providerAndCollectionName) : base(providerAndCollectionName) { }
    public PamelloDatabaseRepositoryAttribute(string providerName, string collectionName) : base(providerName) { }
}
