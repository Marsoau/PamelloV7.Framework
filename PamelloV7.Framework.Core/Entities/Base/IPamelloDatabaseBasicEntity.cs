using PamelloV7.Framework.Core.Entities.Dao;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.Entities.Base;

public interface IPamelloDatabaseBasicEntity : IPamelloBasicEntity
{
    public PamelloBasicDao GetDao();

    public void Save();
}
