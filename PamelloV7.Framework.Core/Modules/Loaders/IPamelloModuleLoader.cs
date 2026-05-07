using System.Reflection;
using PamelloV7.Framework.Core.Modules.Containers;

namespace PamelloV7.Framework.Core.Modules.Loaders;

public interface IPamelloModuleLoader
{
    public List<IPamelloModuleContainer> Containers { get; }
    public PamelloModule? GetAssemblyModule(Assembly assembly);
}
