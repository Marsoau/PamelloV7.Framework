using PamelloV7.Framework.Core.Entities.Dbo;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.Entities.Base;

public interface IPamelloDatabaseBasicEntity : IPamelloBasicEntity
{
    public PamelloBasicDbo GetDbo();

    public void Save();
}
