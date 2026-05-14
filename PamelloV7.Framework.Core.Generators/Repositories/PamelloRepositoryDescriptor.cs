using System.Text;
using Microsoft.CodeAnalysis;
using PamelloV7.Framework.Shared.Generators.Base;

namespace PamelloV7.Framework.Core.Generators.Repositories;

public record PamelloRepositoryDescriptor(
    INamedTypeSymbol InvokingType,
    
    string ProviderName,
    INamedTypeSymbol RepositoryBaseType,
    INamedTypeSymbol PamelloEntityType,
    string? CollectionName,
    bool IsLazy,
    
    StringBuilder DebugOutput
) : PamelloDescriptor(
    InvokingType,
    DebugOutput
) {
    public bool IsDatabaseRepository => CollectionName is not null;
};
