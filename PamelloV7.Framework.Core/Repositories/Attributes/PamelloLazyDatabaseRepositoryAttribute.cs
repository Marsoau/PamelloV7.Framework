using PamelloV7.Framework.Shared.Attributes;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.Repositories.Attributes;

[PamelloRepositoryBaseType(typeof(PamelloDatabaseRepository<,>))]

public class PamelloLazyDatabaseRepositoryAttribute<TPamelloEntity> : PamelloDatabaseRepositoryAttribute<TPamelloEntity>
    where TPamelloEntity : class, IPamelloBasicEntity
{
    public PamelloLazyDatabaseRepositoryAttribute(string providerAndCollectionName) : base(providerAndCollectionName) { }
    public PamelloLazyDatabaseRepositoryAttribute(string providerName, string collectionName) : base(providerName, collectionName) { }
}
