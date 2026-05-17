using PamelloV7.Framework.Shared.Entities.Base;
using PamelloV7.Framework.Shared.Exceptions;

namespace PamelloV7.Framework.Shared.Entities.Containers;

public interface ISafe
{
    int Id { get; }
    public Type EntityType { get; }
    public IPamelloBasicEntity? Entity { get; set; }
}

public static class SafeExtensions
{
    extension(ISafe safe)
    {
        public IPamelloBasicEntity RequiredEntity => safe.Entity
            ?? throw new PamelloException($"Entity with id {safe.Id} is no longer available");
    }
}
