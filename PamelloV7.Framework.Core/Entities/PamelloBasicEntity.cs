using PamelloV7.Framework.Shared.Entities.Base;
using PamelloV7.Framework.Shared.Entities.Dto;

namespace PamelloV7.Framework.Core.Entities;

public abstract class PamelloBasicEntity : IPamelloBasicEntity
{
    public abstract int Id { get; }
    
    public abstract PamelloBasicDto GetDto();
    
    public virtual bool IsDeleted => false;
    public virtual bool IsAvailable() => !IsDeleted;
}
