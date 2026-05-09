using System.Text;
using Microsoft.CodeAnalysis;

namespace PamelloV7.Framework.Shared.Generators.Inheritance;

public record AutoInheritanceDescriptor(
    INamedTypeSymbol ClassType,
    INamedTypeSymbol? InheritanceClass,
    INamedTypeSymbol[] InheritanceInterfaces,
    StringBuilder DebugOutput
);
