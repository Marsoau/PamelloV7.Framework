using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Core.Context;
using PamelloV7.Framework.Core.Data;
using PamelloV7.Framework.Core.Entities.Base;
using PamelloV7.Framework.Core.Entities.Dao;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.Entities;

public abstract class PamelloBasicDatabaseEntity : PamelloBasicEntity, IPamelloDatabaseBasicEntity
{
    protected int _id;
    public override int Id => _id;
    
    public abstract PamelloBasicDao GetDao();

    public virtual void Save() {
        var database = PamelloAppContext.Services.GetService<IDatabaseAccessService>();
        var collection = database?.GetCollectionOfEntity(GetType());
        if (collection is null) return;
        
        var dao = GetDao();
        
        collection.Save(dao);
        
        _id = dao.Id;
    }
}
