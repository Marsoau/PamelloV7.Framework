using System.Reflection;
using PamelloV7.Framework.Core.Modules;
using PamelloV7.Framework.Core.Modules.Loaders;
using PamelloV7.Framework.Core.Services.Base;

namespace PamelloV7.Framework.Core.Logging;

public enum PamelloLogLevel
{
    Log,
    Warning,
    Error,
    Debug,
}

public abstract class PamelloLogger
{
    public IPamelloModuleLoader? Modules { get; set; }
    
    public void Write(
        object? obj = null,
        PamelloLogLevel level = PamelloLogLevel.Log,
        Assembly? assembly = null
    ) {
        assembly ??= Assembly.GetCallingAssembly();
        
        Write(obj, level, Modules?.GetAssemblyModule(assembly));
    }
    public abstract void Write(
        object? obj = null,
        PamelloLogLevel level = PamelloLogLevel.Log,
        PamelloModule? module = null
    );
}