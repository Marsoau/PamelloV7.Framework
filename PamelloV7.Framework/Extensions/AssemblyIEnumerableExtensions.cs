using System.Reflection;
using PamelloV7.Framework.Core.Modules;

namespace PamelloV7.Framework.Extensions;

public static class AssemblyIEnumerableExtensions
{
    extension(IEnumerable<Assembly> assemblies)
    {
        public IEnumerable<Type> SelectManyInNonModuleAssemblies() =>
            assemblies.SelectMany(a => a.GetTypes() is { } types &&
                !types.Any(t => t is { IsClass: true, IsAbstract: false } && t.IsSubclassOf(typeof(PamelloModule))) ? types : []
            );
    }
}
