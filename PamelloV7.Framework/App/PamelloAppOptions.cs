using System.Reflection;
using PamelloV7.Framework.Core.Logging;

namespace PamelloV7.Framework.App;

public record PamelloAppOptions()
{
    public PamelloLogger? Logger { get; init; } = new PamelloConsoleLogger();
    
    public string ReleaseDataPath { get; init; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        Assembly.GetEntryAssembly()?.GetName().Name ?? "UnknownPamelloApp"
    );
    
    public string DebugDataPath { get; init; } = Path.Combine(
        AppContext.BaseDirectory,
        "Data"
    );
    
    public string ReleaseConfigPath { get; init; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        Assembly.GetEntryAssembly()?.GetName().Name ?? "UnknownPamelloApp"
    );
    
    public string DebugConfigPath { get; init; } = Path.Combine(
        AppContext.BaseDirectory,
        "Config"
    );

    public bool UseApi { get; init; } = true;
    public List<string> ApiUrls { get; init; } = [];
}

public static class PamelloAppOptionsExtensions
{
    extension(PamelloAppOptions options)
    {
        public string DataPath =>
        #if DEBUG
            options.DebugDataPath;
        #else
            ReleaseDataPath;
        #endif

        public string ConfigPath =>
        #if DEBUG
            options.DebugConfigPath;
        #else
            ReleaseConfigPath;
        #endif
    }
}
