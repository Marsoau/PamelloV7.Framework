using System.Text;
using Microsoft.CodeAnalysis;

namespace PamelloV7.Framework.Core.Generators.Config;

public record ConfigRootPartDescriptor(
    ITypeSymbol RootNodeClass,
    StringBuilder DebugOutput
);
