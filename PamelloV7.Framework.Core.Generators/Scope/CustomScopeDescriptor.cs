using System.Text;
using Microsoft.CodeAnalysis;
using PamelloV7.Framework.Shared.Generators.Base;

namespace PamelloV7.Framework.Core.Generators.Scope;

public record CustomScopeDescriptor(
    INamedTypeSymbol InvokingType,
    INamedTypeSymbol UserType,
    StringBuilder DebugOutput
) : PamelloDescriptor(
    InvokingType,
    DebugOutput
);
