using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Core.Context;
using PamelloV7.Framework.Core.Data;
using PamelloV7.Framework.Shared.Entities.Base;
using PamelloV7.Framework.Shared.Entities.Dto;

namespace PamelloV7.Framework.Core.Entities;

public abstract class PamelloBasicEntity : IPamelloBasicEntity
{
    public abstract int Id { get; }
    
    public abstract PamelloBasicDto GetDto();
    
    public virtual bool IsDeleted { get; private set; }
    public virtual bool IsAvailable() => !IsDeleted;
    
    public virtual void Delete() {
        IsDeleted = true;
        
        var database = PamelloAppContext.Services.GetService<IDatabaseAccessService>();
        var collection = database?.GetCollectionOfEntity(GetType());

        collection?.Delete(Id);
    }
}
