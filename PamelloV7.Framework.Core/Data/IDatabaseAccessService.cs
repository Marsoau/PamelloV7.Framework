using PamelloV7.Framework.Core.Entities.Dao;
using PamelloV7.Framework.Core.Services.Base;

namespace PamelloV7.Framework.Core.Data;

public interface IDatabaseAccessService : IPamelloService
{
    public IDatabaseCollection<TType> GetCollection<TType>(string name);
    public IDatabaseCollection<PamelloBasicDao>? GetCollectionOfEntity(Type entityType);
}
