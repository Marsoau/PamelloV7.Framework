using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using PamelloV7.Framework.Shared.Generators.Extensions;

namespace PamelloV7.Framework.Shared.Generators.Config;

[Generator]
public class StaticConfigGenerator : IIncrementalGenerator
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

    private static ConfigRootPartDescriptor? GetDescriptor(GeneratorSyntaxContext context, CancellationToken cancellationToken) {
        if (context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken) is not INamedTypeSymbol rootNodeClass) {
            return null;
        }
        if (rootNodeClass.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "ConfigRootAttribute") is not { } attribute) {
            return null;
        }
        
        var debug = new StringBuilder();
        
        debug.AppendLine($"Found {rootNodeClass.Name}");
        
        return new ConfigRootPartDescriptor(
            rootNodeClass,
            debug
        );
    }

    public static string AdjustedName(string name, string end) => name.EndsWith(end) ? name.Substring(0, name.Length - end.Length) : name;
    
    private static string GenerateNode(ITypeSymbol nodeType, int depth, bool isRoot = true) {
        var innerTypes = nodeType.GetTypeMembers().Where(t => !t.IsAbstract && t.Arity == 0).ToList();
        if (!innerTypes.Any()) return $"{GeneratorBase.Tab(depth)}//no inner types";
        
        var sb = new StringBuilder();
        
        sb.AppendLine($"{GeneratorBase.Tab(depth)}public partial class {nodeType.Name} {{");
        
        foreach (var innerType in innerTypes) {
            sb.Append(GeneratorBase.Tab(depth + 1) + $"public {innerType.Name} {AdjustedName(innerType.Name, "Node")} {{ get; set; }}");
            if (innerType.GetMembers().OfType<IPropertySymbol>().Any(p => p.IsRequired)) {
                sb.AppendLine();
            }
            else {
                sb.AppendLine(" = new();");
            }
            sb.AppendLine(GenerateNode(innerType, depth + 1));
        }
        
        sb.AppendLine($"{GeneratorBase.Tab(depth)}}}");
        
        return sb.ToString();
    }
        
    private static void Generate(SourceProductionContext context, ConfigRootPartDescriptor descriptor) {
        var source =
            $$"""
            /* debug output
            {{descriptor.DebugOutput}}
            */
            
            using PamelloV7.Framework.Core.Config.Parts;
            
            {{GeneratorBase.GetNamespaceDeclaration(descriptor.RootNodeClass)}}
            
            //static config part
            public static partial class {{AdjustedName(descriptor.RootNodeClass.Name, "Node")}}Config {
                public static {{descriptor.RootNodeClass.GetFullName()}} Root;
                public static IPamelloConfigPart Part;
            }
            
            //root node
            {{GenerateNode(descriptor.RootNodeClass, 0)}}
            
            """;
        
        context.AddSource($"{AdjustedName(descriptor.RootNodeClass.Name, "Node")}Config.g.cs", SourceText.From(source, Encoding.UTF8));
    }
}
