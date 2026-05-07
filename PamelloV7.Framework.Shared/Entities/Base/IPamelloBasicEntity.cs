using PamelloV7.Framework.Shared.Entities.Dto;

namespace PamelloV7.Framework.Shared.Entities.Base;

public interface IPamelloBasicEntity
{
    public int Id { get; }
    
    public PamelloBasicEntityDto Dto => new() { Id = Id };

    public bool IsDeleted => false;
    public bool IsAvailable() => !IsDeleted;
}
