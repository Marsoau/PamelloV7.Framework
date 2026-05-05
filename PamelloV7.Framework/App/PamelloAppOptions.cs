using PamelloV7.Framework.Core.Logging;

namespace PamelloV7.Framework.App;

public record PamelloAppOptions()
{
    public IPamelloLogger? Logger { get; init; } = new PamelloConsoleLogger();
    
    public bool UseApi { get; init; } = true;
}
