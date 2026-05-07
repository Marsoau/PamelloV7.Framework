using System.Reflection;
using PamelloV7.Framework.Core.Modules;

namespace PamelloV7.Framework.Core.Logging;

public class PamelloConsoleLogger : PamelloLogger
{
    public override void Write(object? obj = null, PamelloLogLevel level = PamelloLogLevel.Log, PamelloModule? module = null) {
        Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} [{module?.ToString() ?? "Server"} | {level}] {obj}");
    }
}
