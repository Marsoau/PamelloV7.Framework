using System.Text;
using Microsoft.CodeAnalysis;
using PamelloV7.Framework.Shared.Generators.Base;

namespace PamelloV7.Framework.Shared.Generators.Inheritance;

public record AutoInheritanceDescriptor(
    INamedTypeSymbol ClassType,
    INamedTypeSymbol? InheritanceClass,
    INamedTypeSymbol[] InheritanceInterfaces,
    StringBuilder DebugOutput
) : PamelloDescriptor(
    ClassType,
    DebugOutput
);
