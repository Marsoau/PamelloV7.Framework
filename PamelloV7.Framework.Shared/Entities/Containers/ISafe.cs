using PamelloV7.Framework.Shared.Entities.Base;
using PamelloV7.Framework.Shared.Exceptions;

namespace PamelloV7.Framework.Shared.Entities.Containers;

public interface ISafe
{
    int Id { get; }
    public IPamelloBasicEntity? Entity { get; set; }
    
    public Type EntityType { get; }
}

