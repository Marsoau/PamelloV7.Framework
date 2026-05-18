using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Shared.Entities.Containers;

public interface ISafeCollection
{
    public IEnumerable<int> Ids { get; }
    public IEnumerable<IPamelloBasicEntity?> Entities { get; }
    public IEnumerable<IPamelloBasicEntity> AvailableEntities { get; }
    
    public Type EntityType { get; }
}
