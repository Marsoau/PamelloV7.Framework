using System.Reflection;

namespace PamelloV7.Framework.Core.Modules.Containers;

public interface IPamelloModuleContainer
{
    public Assembly Assembly { get; }
    public PamelloModule Module { get; }
    public Dictionary<Type, Type?> Services { get; }
    public List<string> Dependencies { get; }
    public Dictionary<string, KeyValuePair<Type, Type>> ConfigTypes { get; }
}

