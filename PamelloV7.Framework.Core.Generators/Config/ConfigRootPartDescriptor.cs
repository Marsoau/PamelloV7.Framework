using System.Text;
using Microsoft.CodeAnalysis;
using PamelloV7.Framework.Shared.Generators.Base;

namespace PamelloV7.Framework.Core.Generators.Config;

public record ConfigRootPartDescriptor(
    INamedTypeSymbol RootNodeClass,
    StringBuilder DebugOutput
) : PamelloDescriptor(
    RootNodeClass,
    DebugOutput
);
