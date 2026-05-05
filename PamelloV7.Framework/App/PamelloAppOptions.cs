namespace PamelloV7.Framework.App;

public record PamelloAppOptions()
{
    public bool UseApi { get; init; } = true;
    public bool UseDatabase { get; init; } = true;
    public bool UseModules { get; init; } = true;
    public bool UseAudioSystem { get; init; } = true;
}
