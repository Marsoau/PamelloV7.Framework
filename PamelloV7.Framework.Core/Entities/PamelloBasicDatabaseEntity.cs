using PamelloV7.Framework.Core.Entities.Base;
using PamelloV7.Framework.Core.Entities.Dbo;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.Entities;

public abstract class PamelloBasicDatabaseEntity : PamelloBasicEntity, IPamelloDatabaseBasicEntity
{
    public abstract PamelloBasicDbo GetDbo();
}
