using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PamelloV7.Framework.Shared.Generators.Base;
using PamelloV7.Framework.Shared.Generators.Extensions;
using PamelloV7.Framework.Shared.Generators.Helpers;

namespace PamelloV7.Framework.Core.Generators.Scope;

[Generator]
public class CustomScopeGenerator : PamelloGenerator<CustomScopeDescriptor>
{
    private const string CustomScopeAttributeName = "CustomScopeAttribute";
    private const string PamelloAppScopeFullName = "global::PamelloV7.Framework.Core.Scope.PamelloAppScope";
    private const string NotFoundExceptionFullName = "global::PamelloV7.Framework.Core.Exceptions.PamelloNoScopeUserException";
    
    protected override bool Predicate(SyntaxNode node, CancellationToken cancellationToken)
        => node is ClassDeclarationSyntax;

    protected override CustomScopeDescriptor? GetDescriptorInternal(
        GeneratorSyntaxContext context,
        INamedTypeSymbol targetType,
        StringBuilder debugOutput
    ) {
        var attribute = targetType.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == CustomScopeAttributeName);
        var userType = attribute?.AttributeClass?.TypeArguments.FirstOrDefault();
        
        if (userType is not INamedTypeSymbol userTypeSymbol) return null;
        
        return new CustomScopeDescriptor(
            targetType,
            userTypeSymbol,
            debugOutput
        );
    }

    protected override void Generate(CustomScopeDescriptor descriptor, StringBuilder generatorSb) {
        var userTypeFullName = descriptor.UserType.GetFullName();
        
        generatorSb.AppendLine(
            SharedHelper.WriteInsideType(
                descriptor.InvokingType,
                "",
                $$"""
                public static void RequireUser() {
                    if (User is null) throw new {{NotFoundExceptionFullName}}();
                }
                
                public static {{userTypeFullName}} RequiredUser => User ?? throw new Exception();
                public static {{userTypeFullName}}? User => {{PamelloAppScopeFullName}}.User as {{descriptor.UserType.GetFullName()}};
                
                public static void SetUserIn({{userTypeFullName}}? user, Action action) => {{PamelloAppScopeFullName}}.SetUserIn(user, action);
                """
            ).ToString()
        );
    }
}
