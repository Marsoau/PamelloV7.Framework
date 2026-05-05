using System.Reflection;
using PamelloV7.Framework.Core.Services.Base;

namespace PamelloV7.Framework.Core.Logging;

public enum PamelloLogLevel
{
    Log,
    Warning,
    Error,
    Debug,
}

public interface IPamelloLogger
{
    public void Write(
        object? obj = null,
        PamelloLogLevel level = PamelloLogLevel.Log,
        Assembly? assembly = null
    );
}
