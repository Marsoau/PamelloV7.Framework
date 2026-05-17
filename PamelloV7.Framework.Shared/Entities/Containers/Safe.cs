using PamelloV7.Framework.Shared.Entities.Base;
using PamelloV7.Framework.Shared.Exceptions;

namespace PamelloV7.Framework.Shared.Entities.Containers;

public static class SafeContainerGetters
{
    public static Func<Type, int, IPamelloBasicEntity?> GetById = (_, _)
        => throw new PamelloException("Getter was not set for safe container");
}

public class Safe<TPamelloEntity> : ISafe
    where TPamelloEntity : class, IPamelloBasicEntity
{
    public int Id { get; set; }
    public Type EntityType => typeof(TPamelloEntity);

    public TPamelloEntity? Entity {
        get => SafeContainerGetters.GetById(EntityType, Id)
            is TPamelloEntity entity && entity.IsAvailable() ? entity : null;
        set => Id = value?.Id ?? 0;
    }
    IPamelloBasicEntity? ISafe.Entity {
        get => Entity;
        set => Entity = value as TPamelloEntity;
    }
    
    public Safe() {
        Id = 0;
    }
    public Safe(int id) {
        Id = id;
    }
}
