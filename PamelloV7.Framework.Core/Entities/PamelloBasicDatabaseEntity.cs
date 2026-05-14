using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Core.Context;
using PamelloV7.Framework.Core.Data;
using PamelloV7.Framework.Core.Entities.Base;
using PamelloV7.Framework.Core.Entities.Dbo;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Core.Entities;

public abstract class PamelloBasicDatabaseEntity : PamelloBasicEntity, IPamelloDatabaseBasicEntity
{
    protected int _id;
    public override int Id => _id;
    
    public abstract PamelloBasicDbo GetDbo();

    public virtual void Save() {
        var database = PamelloAppContext.Services.GetService<IDatabaseAccessService>();
        var collection = database?.GetCollectionOfEntity(GetType());
        if (collection is null) return;
        
        var dbo = GetDbo();
        
        collection.Save(dbo);
        
        _id = dbo.Id;
    }
}
