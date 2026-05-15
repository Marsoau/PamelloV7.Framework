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
    
    private const string PamelloRepositoryDatabaseRepositoryAttributeName = "PamelloDatabaseRepositoryAttribute";
    private const string PamelloRepositoryLazyDatabaseRepositoryAttributeName = "PamelloLazyDatabaseRepositoryAttribute";
    
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
        
        var isDatabaseRepository = SharedHelper.CheckTypeName(repositoryAttribute.AttributeClass, PamelloRepositoryDatabaseRepositoryAttributeName);
        var isLazy = SharedHelper.CheckTypeName(repositoryAttribute.AttributeClass, PamelloRepositoryLazyDatabaseRepositoryAttributeName);
        
        var providerName = repositoryAttribute.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString();
        var collectionName = repositoryAttribute.ConstructorArguments.ElementAtOrDefault(1).Value?.ToString();

        if (isDatabaseRepository) collectionName ??= providerName;
        
        if (providerName is null) return null;
        
        var baseTypeAttribute = repositoryAttribute.AttributeClass?.GetAttributes()
            .FirstOrDefault(a => SharedHelper.CheckTypeName(a.AttributeClass, PamelloRepositoryBaseTypeAttributeName));

        if (baseTypeAttribute?.ConstructorArguments.ElementAtOrDefault(0).Value is not INamedTypeSymbol repositoryBaseType) return null;
        
        debugOutput.AppendLine($"Repository base type: {repositoryBaseType.Name}");

        return new PamelloRepositoryDescriptor(
            targetType,
            
            providerName,
            repositoryBaseType,
            pamelloEntityType,
            collectionName,
            isLazy,
            
            debugOutput
        );
    }

    private static string GetRepositoryInheritancePartSource(PamelloRepositoryDescriptor descriptor) {
        var inheritancePartSb = new StringBuilder();

        inheritancePartSb.Append($": {descriptor.RepositoryBaseType.GetFullName()}");

        if (descriptor.IsDatabaseRepository) {
            inheritancePartSb.Insert(inheritancePartSb.Length - 2, descriptor.PamelloEntityType.GetFullName());
            inheritancePartSb.Insert(inheritancePartSb.Length - 1, $"{descriptor.PamelloEntityType.GetFullName()}.Dao");
        }
        else {
            inheritancePartSb.Insert(inheritancePartSb.Length - 1, descriptor.PamelloEntityType.GetFullName());
        }
        
        return inheritancePartSb.ToString();
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

    private static string GetRepositoryCollectionPropertySource(PamelloRepositoryDescriptor descriptor) {
        if (!descriptor.IsDatabaseRepository) return "//no collection property required for non-database repositories";
        
        return $"public override string CollectionName => \"{descriptor.CollectionName}\";";
    }

    protected override void Generate(PamelloRepositoryDescriptor descriptor, StringBuilder generatorSb) {
        descriptor.DebugOutput.AppendLine($"Is database repository: {descriptor.IsDatabaseRepository}");
        descriptor.DebugOutput.AppendLine($"Is lazy: {descriptor.IsLazy}");
        
        generatorSb.AppendLine(
            SharedHelper.WriteInsideType(
                descriptor.InvokingType,
                GetRepositoryInheritancePartSource(descriptor),
                $$"""
                {{GetRepositoryCollectionPropertySource(descriptor)}}
                {{GetRepositoryConstructorSource(descriptor)}}
                """
            ).ToString()
        );
    }
}
