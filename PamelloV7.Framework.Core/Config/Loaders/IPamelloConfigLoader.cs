using PamelloV7.Framework.Core.Config.Parts;

namespace PamelloV7.Framework.Core.Config.Loaders;

public interface IPamelloConfigLoader
{
    public List<IPamelloConfigPart> Parts { get; }
}
