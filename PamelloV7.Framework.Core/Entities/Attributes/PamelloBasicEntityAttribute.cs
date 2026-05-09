using PamelloV7.Framework.Shared.Attributes;

namespace PamelloV7.Framework.Core.Entities.Attributes;

[AttributeUsage(AttributeTargets.Class)]

[AutoInherit(typeof(PamelloBasicEntity), [])]

public class PamelloBasicEntityAttribute : Attribute
{
    
}
