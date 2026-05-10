using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using PamelloV7.Framework.Shared.Generators.Extensions;
using PamelloV7.Framework.Shared.Generators.Helpers;

namespace PamelloV7.Framework.Shared.Generators.Inheritance;

[Generator]
public class AutoInheritanceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 }, 
                transform: GetDescriptor
            )
            .Where(static m => m is not null);

        context.RegisterSourceOutput(classDeclarations, (c, d) => Generate(c, d!));
    }

    private static HashSet<INamedTypeSymbol> GetAutoInheritanceTypes(
        INamedTypeSymbol classSymbol,
        bool isFirstClass = true,
        HashSet<INamedTypeSymbol>? visited = null,
        HashSet<INamedTypeSymbol>? yielded = null
    ) {
        visited ??= new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
        yielded ??= new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
        
        if (!visited.Add(classSymbol)) return yielded;

        const string attributeName = "AutoInheritAttribute";

        foreach (var attribute in classSymbol.GetAttributes()) {
            var attributeClass = attribute?.AttributeClass;
            if (attributeClass is null) continue;

            if (attributeClass.Name == attributeName) {
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

    public static AttributeData? GetAttributeInClass(INamedTypeSymbol classSymbol, string attributeName, int depth = 0) {
        //if (depth >= 3) return null;
        
        foreach (var attribute in classSymbol.GetAttributes()) {
            var attributeClass = attribute?.AttributeClass;
            if (attributeClass is null) continue;
            
            if (attributeClass.Name == attributeName) return attribute;
            
            var data = GetAttributeInClass(attributeClass, attributeName, depth + 1);
            if (data is not null) return data;
        }

        if (classSymbol.BaseType is not null) {
            return GetAttributeInClass(classSymbol.BaseType, attributeName, depth + 1);
        }
        
        return null;
    }
    
    private static AutoInheritanceDescriptor? GetDescriptor(GeneratorSyntaxContext context, CancellationToken cancellationToken) {
        if (context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken) is not INamedTypeSymbol classType) {
            return null;
        }
        if (classType.BaseType is null || classType.BaseType.Name != "Object") {
            return null;
        }
        
        var types = GetAutoInheritanceTypes(classType).ToList();
        if (types.Count == 0) return null;
        
        var debug = new StringBuilder();
        
        debug.AppendLine($"Found in {classType.Name}");

        var hasClassFirst = types.FirstOrDefault() is { IsImplicitClass: true };
        
        return new AutoInheritanceDescriptor(
            classType,
            hasClassFirst
                ? types.First()
                : null,
            types.Skip(hasClassFirst ? 1 : 0).ToArray(),
            debug
        );
    }

    private static void Generate(SourceProductionContext context, AutoInheritanceDescriptor descriptor) {
        var classNamespace = SharedHelper.GetNamespace(descriptor.ClassType);
        
        var types = descriptor.InheritanceInterfaces.Prepend(descriptor.InheritanceClass)
            .OfType<ITypeSymbol>()
            .ToArray();
        
        var sb = SharedHelper.WriteInsideClasses(
            descriptor.ClassType,
            $" : {string.Join(", ", types.Select(t => t.GetFullName()))}",
            "//nothing"
        );

        var source =
            $$"""
              /* debug output
              {{descriptor.DebugOutput}}
              */
              
              namespace {{classNamespace}};
              
              {{sb}}
              """;
        
        context.AddSource($"{descriptor.ClassType.Name}.AutoInheritance.g.cs", SourceText.From(source, Encoding.UTF8));
    }
}
