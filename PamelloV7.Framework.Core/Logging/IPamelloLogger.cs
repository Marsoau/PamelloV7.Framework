using System.Reflection;
using PamelloV7.Framework.Core.Modules.Base;
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
    public void Write(
        object? obj = null,
        PamelloLogLevel level = PamelloLogLevel.Log,
        Assembly? assembly = null
    ) {
        assembly ??= Assembly.GetCallingAssembly();

        var module = (IPamelloModule?)null; //todo get module here later
        
        Write(obj, level, module);
    }
    public abstract void Write(
        object? obj = null,
        PamelloLogLevel level = PamelloLogLevel.Log,
        IPamelloModule? module = null
    );
}