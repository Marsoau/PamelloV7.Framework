using System.Text;
using Microsoft.CodeAnalysis;
using PamelloV7.Framework.Shared.Generators.Base;

namespace PamelloV7.Framework.Shared.Generators.Variants;

public record VariantDescriptor(
    IMethodSymbol ParentMethod,
    IParameterSymbol Parameter,
    IMethodSymbol VariantMethod
);

public record ParameterVariantsDescriptor(
    IMethodSymbol Method,
    IParameterSymbol Parameter,
    List<VariantDescriptor?> Variants,
    bool IsRequired
);

public record VariantsDescriptor(
    INamedTypeSymbol Class,
    Dictionary<IMethodSymbol, List<ParameterVariantsDescriptor>> ParametersWithVariants,
    StringBuilder DebugOutput
) : PamelloDescriptor(
    Class,
    DebugOutput
);
