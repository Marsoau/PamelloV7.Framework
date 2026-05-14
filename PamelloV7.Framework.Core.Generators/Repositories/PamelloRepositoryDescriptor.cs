using System.Text;
using Microsoft.CodeAnalysis;
using PamelloV7.Framework.Shared.Generators.Base;

namespace PamelloV7.Framework.Core.Generators.Repositories;

public record PamelloRepositoryDescriptor(
    INamedTypeSymbol InvokingType,
    INamedTypeSymbol RepositoryBaseType,
    INamedTypeSymbol PamelloEntityType,
    StringBuilder DebugOutput
) : PamelloDescriptor(
    InvokingType,
    DebugOutput
);
