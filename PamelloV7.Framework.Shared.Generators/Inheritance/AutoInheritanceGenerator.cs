using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PamelloV7.Framework.Shared.Generators.Base;
using PamelloV7.Framework.Shared.Generators.Extensions;
using PamelloV7.Framework.Shared.Generators.Helpers;

namespace PamelloV7.Framework.Shared.Generators.Inheritance;

[Generator]
public class AutoInheritanceGenerator : PamelloGenerator<AutoInheritanceDescriptor>
{
    private const string AttributeName = "AutoInheritAttribute";
    
    protected override bool Predicate(SyntaxNode node, CancellationToken cancellationToken)
        => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };
    
    private static HashSet<INamedTypeSymbol> GetAutoInheritanceTypes(
        INamedTypeSymbol classSymbol,
        bool isFirstClass = true,
        HashSet<INamedTypeSymbol>? visited = null,
        HashSet<INamedTypeSymbol>? yielded = null
    ) {
        visited ??= new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
        yielded ??= new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
        
        if (!visited.Add(classSymbol)) return yielded;

        foreach (var attribute in classSymbol.GetAttributes()) {
            var attributeClass = attribute?.AttributeClass;
            if (attributeClass is null) continue;

            if (attributeClass.Name == AttributeName) {
                if (isFirstClass && attribute?.ConstructorArguments.ElementAtOrDefault(0).Value is INamedTypeSymbol inheritanceClass) {
                    yielded.Add(inheritanceClass);
                    isFirstClass = false;
                }

                foreach (var interfaceType in attribute?.ConstructorArguments.ElementAtOrDefault(1).Values
                        .Select(v => v.Value)
                        .OfType<INamedTypeSymbol>() ?? []
                ) {
                    yielded.Add(interfaceType);
                }
            }

            GetAutoInheritanceTypes(attributeClass, isFirstClass, visited, yielded);
        }

        if (classSymbol.BaseType is null || classSymbol.BaseType.Name == "Object") return yielded;
        
        GetAutoInheritanceTypes(classSymbol.BaseType, isFirstClass, visited, yielded);
        
        return yielded;
    }

    protected override AutoInheritanceDescriptor? GetDescriptorInternal(
        GeneratorSyntaxContext context,
        INamedTypeSymbol targetType,
        StringBuilder debugOutput
    ) {
        if (targetType.BaseType is null || targetType.BaseType.Name != "Object") {
            return null;
        }
        
        var types = GetAutoInheritanceTypes(targetType).ToList();
        if (types.Count == 0) return null;
        
        debugOutput.AppendLine($"Found in {targetType.Name}");

        var hasClassFirst = types.FirstOrDefault() is { TypeKind: TypeKind.Class };

        var inheritanceClass = hasClassFirst
            ? types.First()
            : null;

        if (inheritanceClass is { IsUnboundGenericType: true } ) {
            //todo replace first attribute for actual search of a closest compatible type argument
            
            var firstAttribute = targetType.GetAttributes().FirstOrDefault(
                fa => fa.AttributeClass?.GetAttributes().Any(a => a.AttributeClass?.Name == AttributeName) ?? false
            );
            
            inheritanceClass = inheritanceClass.OriginalDefinition.Construct(
                (firstAttribute?.AttributeClass?.TypeArguments ?? []).ToArray()
            );
        }
        
        return new AutoInheritanceDescriptor(
            targetType,
            inheritanceClass,
            types.Skip(hasClassFirst ? 1 : 0).ToArray(),
            debugOutput
        );
    }

    protected override void Generate(AutoInheritanceDescriptor descriptor, StringBuilder generatorSb) {
        var types = descriptor.InheritanceInterfaces.Prepend(descriptor.InheritanceClass)
            .OfType<ITypeSymbol>()
            .ToArray();

        generatorSb.AppendLine(
            SharedHelper.WriteInsideType(
                descriptor.ClassType,
                types.Length > 0
                    ? $" : {string.Join(", ", types.Select(t => t.GetFullName()))}"
                    : "",
                "//nothing"
            ).ToString()
        );
    }
}
