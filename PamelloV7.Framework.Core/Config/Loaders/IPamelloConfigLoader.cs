using PamelloV7.Framework.Core.Config.Parts;

namespace PamelloV7.Framework.Core.Config.Loaders;

public interface IPamelloConfigLoader
{
    public static string DefaultDataPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "PamelloV7"
    );
    
    public static string DefaultConfigFilePath = Path.Combine(DefaultDataPath, "Config", "config.jsonc");
    
    public List<IPamelloConfigPart> Parts { get; }
}
