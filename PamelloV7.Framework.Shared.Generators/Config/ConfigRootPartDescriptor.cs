using System.Text;
using Microsoft.CodeAnalysis;

namespace PamelloV7.Framework.Shared.Generators.Config;

public record ConfigRootPartDescriptor(
    ITypeSymbol ClassType,
    StringBuilder DebugOutput
);
