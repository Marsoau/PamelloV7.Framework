using System.Text;
using Microsoft.CodeAnalysis;
using PamelloV7.Framework.Shared.Generators.Base;

namespace PamelloV7.Framework.Shared.Generators.Entities;

public record PamelloEntityUpdatablePropertyDescriptor(
    IPropertySymbol Property
);
    
public record PamelloEntityDescriptor(
    INamedTypeSymbol InvokingType,
    bool NeedsIdGenerated,
    bool IsDatabaseEntity,
    List<PamelloEntityUpdatablePropertyDescriptor> UpdatableProperties,
    StringBuilder DebugOutput
) : PamelloDescriptor(
    InvokingType,
    DebugOutput
);
