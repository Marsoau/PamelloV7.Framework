using System.Text;
using Microsoft.CodeAnalysis;
using PamelloV7.Framework.Shared.Generators.Extensions;

namespace PamelloV7.Framework.Shared.Generators.Base;

public abstract class PamelloGenerator<TDescriptor> : IIncrementalGenerator
    where TDescriptor : PamelloDescriptor
{
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: Predicate, 
                transform: GetDescriptor
            )
            .Where(static m => m is not null);

        context.RegisterSourceOutput(classDeclarations, (c, d) => Generate(c, d!));
    }

    protected abstract bool Predicate(SyntaxNode node, CancellationToken cancellationToken);
    
    protected abstract TDescriptor? GetDescriptorInternal(GeneratorSyntaxContext context, INamedTypeSymbol targetType, StringBuilder debugOutput);
    protected virtual TDescriptor? GetDescriptor(GeneratorSyntaxContext context, CancellationToken cancellationToken) {
        if (context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken) is not INamedTypeSymbol targetType) {
            return null;
        }
        
        var debug = new StringBuilder();
        
        debug.AppendLine($"Target type: {targetType.Name} | {targetType.GetFullName()}");
        
        return GetDescriptorInternal(context, targetType, debug);
    }

    protected abstract void Generate(TDescriptor descriptor, StringBuilder generatorSb);
    protected virtual void Generate(SourceProductionContext context, TDescriptor descriptor) {
        var sb = new StringBuilder();
        
        Generate(descriptor, sb);
        
        sb.AppendLine();
        sb.AppendLine(
            $"""
            
            /* Debug Output
            {descriptor.DebugOutput}
            */
            """
        );
        
        context.AddSource($"{descriptor.InvokingType.Name}.{GetType().Name}.{(uint)Guid.NewGuid().GetHashCode()}.g.cs", sb.ToString());
    }
}
