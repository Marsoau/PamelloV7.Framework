using System.Reflection;

namespace PamelloV7.Framework.Core.Logging;

public static class PamelloOutput
{
    public static PamelloLogger? Logger { get; set; }
    
    public static void Write(object? obj = null, PamelloLogLevel level = PamelloLogLevel.Log)
        => Logger?.Write(obj, level, Assembly.GetCallingAssembly());
}
