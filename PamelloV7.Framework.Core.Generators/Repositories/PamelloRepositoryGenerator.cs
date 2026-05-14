using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PamelloV7.Framework.Shared.Generators.Base;
using PamelloV7.Framework.Shared.Generators.Extensions;
using PamelloV7.Framework.Shared.Generators.Helpers;

namespace PamelloV7.Framework.Core.Generators.Repositories;

[Generator]
public class PamelloRepositoryGenerator : PamelloGenerator<PamelloRepositoryDescriptor>
{
    private const string PamelloRepositoryAttributeName = "PamelloRepositoryAttribute";
    private const string PamelloRepositoryBaseTypeAttributeName = "PamelloRepositoryBaseTypeAttribute";
    
    protected override bool Predicate(SyntaxNode node, CancellationToken cancellationToken)
        => node is ClassDeclarationSyntax;

    protected override PamelloRepositoryDescriptor? GetDescriptorInternal(
        GeneratorSyntaxContext context,
        INamedTypeSymbol targetType,
        StringBuilder debugOutput
    ) {
        var repositoryAttribute = targetType.GetAttributes()
            .FirstOrDefault(a => SharedHelper.CheckTypeName(a.AttributeClass, PamelloRepositoryAttributeName));
        
        if (repositoryAttribute?.AttributeClass?.TypeArguments.FirstOrDefault() is not INamedTypeSymbol pamelloEntityType) return null;
        
        var baseTypeAttribute = repositoryAttribute.AttributeClass?.GetAttributes()
            .FirstOrDefault(a => SharedHelper.CheckTypeName(a.AttributeClass, PamelloRepositoryBaseTypeAttributeName));

        if (baseTypeAttribute?.ConstructorArguments.ElementAtOrDefault(0).Value is not INamedTypeSymbol repositoryBaseType) return null;
        
        debugOutput.AppendLine($"Repository base type: {repositoryBaseType.Name}");

        return new PamelloRepositoryDescriptor(
            targetType,
            repositoryBaseType,
            pamelloEntityType,
            debugOutput
        );
    }

    private static string GetRepositoryConstructorSource(PamelloRepositoryDescriptor descriptor) {
        if (descriptor.InvokingType.InstanceConstructors.Any(c => !c.IsImplicitlyDeclared)) {
            return "//repository has an explicit constructor, automatic one will not be generated";
        }
        
        var baseConstructor = descriptor.RepositoryBaseType.OriginalDefinition.InstanceConstructors.FirstOrDefault(c => !c.IsImplicitlyDeclared);
        if (baseConstructor is null) return "//no explicit constructor found in base type";
        
        return $"public {descriptor.InvokingType.Name}(\n{
            string.Join(",\n", baseConstructor.Parameters.Select(p => $"{SharedHelper.Tab(1)}{p.Type.GetFullName()} {p.Name}"))
        }\n) : base({
            string.Join(", ", baseConstructor.Parameters.Select(p => p.Name))
        }) {{ }}";
    }

    protected override void Generate(PamelloRepositoryDescriptor descriptor, StringBuilder generatorSb) {
        var inheritancePartSb = new StringBuilder();

        inheritancePartSb.Append($": {descriptor.RepositoryBaseType.GetFullName()}");
        inheritancePartSb.Insert(inheritancePartSb.Length - 1, descriptor.PamelloEntityType.GetFullName());

        generatorSb.AppendLine(
            SharedHelper.WriteInsideType(
                descriptor.InvokingType,
                inheritancePartSb.ToString(),
                $$"""
                {{GetRepositoryConstructorSource(descriptor)}}
                """
            ).ToString()
        );
    }
}
