using System.Text;
using Microsoft.CodeAnalysis;

namespace PamelloV7.Framework.Shared.Generators.Base;

public record PamelloDescriptor(
    INamedTypeSymbol InvokingType,
    StringBuilder DebugOutput
);
