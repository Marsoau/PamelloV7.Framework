using PamelloV7.Framework.Core.Entities.Base;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.Entities;

public abstract class PamelloDatabaseBasicEntity : PamelloBasicEntity, IPamelloDatabaseBasicEntity
{
    public abstract DatabaseEntityData GetData();
}
