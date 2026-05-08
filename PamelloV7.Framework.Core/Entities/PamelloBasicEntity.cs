using PamelloV7.Framework.Shared.Entities.Base;
using PamelloV7.Framework.Shared.Entities.Dto;

namespace PamelloV7.Framework.Core.Entities;

public abstract class PamelloBasicEntity : IPamelloBasicEntity
{
    public int Id { get; init; }

    public abstract PamelloBasicEntityDto GetDto();
    
    public virtual bool IsDeleted => false;
    public virtual bool IsAvailable() => !IsDeleted;
}
