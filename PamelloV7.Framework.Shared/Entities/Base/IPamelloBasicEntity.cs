using PamelloV7.Framework.Shared.Entities.Dto;

namespace PamelloV7.Framework.Shared.Entities.Base;

public interface IPamelloBasicEntity
{
    public int Id { get; }

    public PamelloBasicDto GetDto();

    public bool IsDeleted { get; }
    public bool IsAvailable();
    
    public void Delete();
}
