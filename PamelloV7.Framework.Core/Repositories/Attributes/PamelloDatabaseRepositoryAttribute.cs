using PamelloV7.Framework.Shared.Attributes;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.Repositories.Attributes;

public interface IPamelloDatabaseRepositoryAttribute : IPamelloRepositoryAttribute
{
    public string CollectionName { get; }
}

[AttributeUsage(AttributeTargets.Class)]

[PamelloRepositoryBaseType(typeof(PamelloDatabaseRepository<,>))]

public class PamelloDatabaseRepositoryAttribute<TPamelloEntity> : PamelloRepositoryAttribute<TPamelloEntity>, IPamelloDatabaseRepositoryAttribute
    where TPamelloEntity : class, IPamelloBasicEntity
{
    public string CollectionName { get; }

    public PamelloDatabaseRepositoryAttribute(string providerAndCollectionName) : base(providerAndCollectionName) {
        CollectionName = providerAndCollectionName;
    }

    public PamelloDatabaseRepositoryAttribute(string providerName, string collectionName) : base(providerName) {
        CollectionName = collectionName;
    }
}
