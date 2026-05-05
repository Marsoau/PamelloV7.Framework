using System.Reflection;

namespace PamelloV7.Framework.Core.Logging;

public class PamelloConsoleLogger : IPamelloLogger
{
    public void Write(object? obj = null, PamelloLogLevel level = PamelloLogLevel.Log, Assembly? assembly = null) {
        assembly ??= Assembly.GetCallingAssembly();
        
        Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} [{null ?? "Server"} | {level}] {obj}");
    }
}
