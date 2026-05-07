using System.Reflection;

namespace PamelloV7.Framework.Core.Modules.Containers;

public interface IPamelloModuleContainer
{
    public Assembly[] Assemblies { get; }
    public Assembly ModuleAssembly { get; }
    public PamelloModule Module { get; }
    public Dictionary<Type, Type?> Services { get; }
    public Dictionary<string, KeyValuePair<Type, Type>> ConfigTypes { get; }
}

