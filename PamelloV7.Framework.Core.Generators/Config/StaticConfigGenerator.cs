using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using PamelloV7.Framework.Shared.Generators.Base;
using PamelloV7.Framework.Shared.Generators.Extensions;
using PamelloV7.Framework.Shared.Generators.Helpers;

namespace PamelloV7.Framework.Core.Generators.Config;

[Generator]
public class StaticConfigGenerator : PamelloGenerator<ConfigRootPartDescriptor>
{
    protected override bool Predicate(SyntaxNode node, CancellationToken cancellationToken)
        => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };

    protected override ConfigRootPartDescriptor? GetDescriptorInternal(
        GeneratorSyntaxContext context,
        INamedTypeSymbol rootNodeClass,
        StringBuilder debugOutput
    ) {
        if (rootNodeClass.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "ConfigRootAttribute") is not { } attribute) {
            return null;
        }
        
        debugOutput.AppendLine($"Found {rootNodeClass.Name}");
        
        return new ConfigRootPartDescriptor(
            rootNodeClass,
            debugOutput
        );
    }

    public static string AdjustedName(string name, string end) => name.EndsWith(end) ? name.Substring(0, name.Length - end.Length) : name;
    
    private static string GenerateNode(ITypeSymbol nodeType, int depth, bool isRoot = true) {
        var innerTypes = nodeType.GetTypeMembers().Where(t => !t.IsAbstract && t.Arity == 0).ToList();
        if (!innerTypes.Any()) return $"{SharedHelper.Tab(depth)}//no inner types";
        
        var sb = new StringBuilder();
        
        sb.AppendLine($"{SharedHelper.Tab(depth)}public partial class {nodeType.Name} {{");
        
        foreach (var innerType in innerTypes) {
            sb.Append(SharedHelper.Tab(depth + 1) + $"public {innerType.Name} {AdjustedName(innerType.Name, "Node")} {{ get; set; }}");
            if (innerType.GetMembers().OfType<IPropertySymbol>().Any(p => p.IsRequired)) {
                sb.AppendLine();
            }
            else {
                sb.AppendLine(" = new();");
            }
            sb.AppendLine(GenerateNode(innerType, depth + 1));
        }
        
        sb.AppendLine($"{SharedHelper.Tab(depth)}}}");
        
        return sb.ToString();
    }
        

    protected override void Generate(ConfigRootPartDescriptor descriptor, StringBuilder generatorSb) {
        var source =
            $$"""
            using PamelloV7.Framework.Core.Config.Parts;
            
            {{SharedHelper.GetNamespaceDeclaration(descriptor.RootNodeClass)}}
            
            //static config part
            public static partial class {{AdjustedName(descriptor.RootNodeClass.Name, "Node")}}Config {
                public static {{descriptor.RootNodeClass.GetFullName()}} Root;
                public static IPamelloConfigPart Part;
            }
            
            //root node
            {{GenerateNode(descriptor.RootNodeClass, 0)}}
            
            """;
        
        generatorSb.AppendLine(
            source
        );
    }
}
