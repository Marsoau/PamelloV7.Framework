namespace PamelloV7.Framework.Core.Config.Parts;

public interface IPamelloConfigPreInitializer
{
    public string PropertyPath { get; }
    public Type PropertyType { get; }

    public object? Value { get; set; }
    public bool IsPreInitialized { get; }
}
