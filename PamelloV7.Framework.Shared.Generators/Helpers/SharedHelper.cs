using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace PamelloV7.Framework.Shared.Generators.Helpers;

public static class SharedHelper
{
    public static string Tab(int count) => string.Join("", Enumerable.Repeat("    ", count));

    public static AttributeData? GetAttributeByName(
        INamedTypeSymbol? typeSymbol,
        string attributeName,
        bool goIntoBaseTypes = false
    ) {
        if (typeSymbol is null) return null;
        
        if (typeSymbol.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == attributeName) is { } attribute) {
            return attribute;
        }
        
        return goIntoBaseTypes && typeSymbol.BaseType is not null && typeSymbol.BaseType.Name != "Object"
            ? GetAttributeByName(typeSymbol.BaseType, attributeName, false)
            : null;
    }
    
    public static bool CheckTypeName(
        INamedTypeSymbol? typeSymbol,
        string name,
        bool goIntoBaseTypes = true
    ) {
        if (typeSymbol is null) return false;
        if (typeSymbol.Name == name) return true;
        
        return
            goIntoBaseTypes &&
            typeSymbol.BaseType is not null &&
            typeSymbol.BaseType.Name != "Object" &&
            CheckTypeName(
                typeSymbol.BaseType,
                name
            );
    }
    
    public static string GetNamespace(ITypeSymbol classSymbol) {
        return classSymbol.ContainingNamespace.IsGlobalNamespace 
            ? string.Empty 
            : classSymbol.ContainingNamespace.ToDisplayString();
    }

    public static string GetNamespaceDeclaration(ITypeSymbol classSymbol, string? innerText = null) {
        var classNamespace = classSymbol.ContainingNamespace.IsGlobalNamespace 
            ? string.Empty 
            : classSymbol.ContainingNamespace.ToDisplayString();
        
        if (string.IsNullOrEmpty(classNamespace)) return "";
        if (innerText is not null) return $"namespace {classNamespace} {{\n\t{innerText}\n}}";
        
        return $"namespace {classNamespace};";
    }

    public static IEnumerable<ITypeSymbol> GetContainingTypes(ITypeSymbol classSymbol) {
        while (true) {
            var containingType = classSymbol.ContainingType;
            if (containingType is null) yield break;
            
            yield return containingType;

            classSymbol = containingType;
        }
    }

    public static void WriteInsideType(ITypeSymbol[] classTypes, string inheritancePart, string innerPart, bool useBodyNamespace, StringBuilder sb, int tabs, int depth) {
        if (classTypes.Length == 0) return;

        sb.Append($"{Tab(tabs)}namespace {GetNamespace(classTypes[depth])}");
        if (useBodyNamespace) {
            sb.AppendLine(" {");
            tabs++;
        }
        else {
            sb.AppendLine(";");
            sb.AppendLine();
        }
    
        var namedType = classTypes[depth] as INamedTypeSymbol;
        var typeParams = namedType?.TypeParameters.Length > 0
            ? $"<{string.Join(", ", namedType.TypeParameters.Select(tp => tp.Name))}>"
            : "";
        var constraints = namedType?.TypeParameters.Length > 0
            ? string.Join(" ", namedType.TypeParameters.Select(GetFullyQualifiedConstraints).Where(c => c.Length > 0))
            : "";
    
        sb.Append($"{Tab(tabs)}{GetTypeModifiers(classTypes[depth])} class {classTypes[depth].Name}{typeParams} ");
    
        if (depth + 1 >= classTypes.Length) {
            var combined = inheritancePart + (constraints.Length > 0 ? $"{constraints} " : "");
            sb.AppendLine($"{combined}{(
                combined.Length == 0 || combined.LastOrDefault() != ' ' ? " {" : "{"
            )}");

            sb.Append(Tab(tabs + 1));
            sb.AppendLine(innerPart.Replace("\n", $"\n{Tab(tabs + 1)}"));
        
            sb.AppendLine($"{Tab(tabs)}}}");
            return;
        }

        sb.AppendLine(constraints.Length > 0 ? $"{constraints} {{" : $"{{");
        WriteInsideType(classTypes, inheritancePart, innerPart, false, sb, tabs + 1, depth + 1);
        sb.AppendLine($"{Tab(tabs)}}}");
    
        if (useBodyNamespace) {
            sb.AppendLine($"{Tab(tabs - 1)}}}");
        }
    }
    
    public static StringBuilder WriteInsideType(ITypeSymbol type, string inheritancePart, string innerPart, bool useBodyNamespace = false) {
        var sb = new StringBuilder();
        
        var containingTypes = GetContainingTypes(type).ToArray();
        WriteInsideType([..containingTypes, type], inheritancePart, innerPart, useBodyNamespace, sb, 0, 0);
        
        return sb;
    }
    
    public static bool HasMethod(ITypeSymbol? type, string methodName, StringBuilder? debug = null) {
        while (true) {
            if (type is null) return false;

            debug?.AppendLine($"Checking: {type.Name}");
            foreach (var member in type.GetMembers().OfType<IMethodSymbol>()) {
                debug?.AppendLine($"| {member.Name}");
            }
            
            if (type.GetMembers().OfType<IMethodSymbol>().Any(m => m.Name == methodName))
                return true;

            type = type.BaseType;
        }
    }

    public static string GetFullyQualifiedConstraints(IMethodSymbol methodSymbol) {
        if (methodSymbol.TypeParameters.Length == 0)
            return string.Empty;

        var constraints = methodSymbol.TypeParameters.Select(GetFullyQualifiedConstraints);
        return string.Join(" ", constraints);
    }
    public static string GetFullyQualifiedConstraints(ITypeParameterSymbol typeParam) {
        var constraints = new List<string>();

        if (typeParam.HasReferenceTypeConstraint) {
            constraints.Add(typeParam.ReferenceTypeConstraintNullableAnnotation == NullableAnnotation.Annotated ? "class?" : "class");
        }
        else if (typeParam.HasValueTypeConstraint) {
            constraints.Add("struct");
        }
        else if (typeParam.HasUnmanagedTypeConstraint) {
            constraints.Add("unmanaged");
        }
        else if (typeParam.HasNotNullConstraint) {
            constraints.Add("notnull");
        }

        foreach (ITypeSymbol typeConstraint in typeParam.ConstraintTypes) {
            constraints.Add(typeConstraint.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        }

        if (typeParam.HasConstructorConstraint) {
            constraints.Add("new()");
        }
        if (constraints.Count > 0) {
            return $"where {typeParam.Name} : {string.Join(", ", constraints)}";
        }

        return "";
    }
    
    public static string GetSymbolModifiers(ISymbol symbol) {
        var modifiers = new List<string>();

        var accessibility = symbol.DeclaredAccessibility switch {
            Accessibility.Public => "public",
            Accessibility.Internal => "internal",
            Accessibility.Protected => "protected",
            Accessibility.ProtectedAndInternal => "private protected",
            Accessibility.ProtectedOrInternal => "protected internal",
            Accessibility.Private => "private",
            _ => ""
        };

        if (!string.IsNullOrEmpty(accessibility))
            modifiers.Add(accessibility);
        if (symbol.IsStatic)
            modifiers.Add("static");
        if (symbol.IsAbstract)
            modifiers.Add("abstract");
        if (symbol.IsVirtual)
            modifiers.Add("virtual");
        if (symbol.IsOverride)
            modifiers.Add("override");
        if (symbol.IsExtern)
            modifiers.Add("extern");

        return string.Join(" ", modifiers);
    }
    
    // New helper — mirrors GetMethodModifiers but for types
    public static string GetTypeModifiers(ITypeSymbol typeSymbol) {
        var modifiers = new List<string>();

        var accessibility = typeSymbol.DeclaredAccessibility switch {
            Accessibility.Public             => "public",
            Accessibility.Internal           => "internal",
            Accessibility.Protected          => "protected",
            Accessibility.ProtectedAndInternal  => "private protected",
            Accessibility.ProtectedOrInternal   => "protected internal",
            Accessibility.Private            => "private",
            _                                => ""
        };

        if (!string.IsNullOrEmpty(accessibility))
            modifiers.Add(accessibility);
        if (typeSymbol.IsStatic)
            modifiers.Add("static");

        modifiers.Add("partial"); // always partial — we're generating into it

        return string.Join(" ", modifiers);
    }
    
    /// <summary>
    /// Checks if a type is nullable, either directly (e.g. string?) or inside a Task (e.g. Task<string?>).
    /// </summary>
    public static bool IsNullable(ITypeSymbol typeSymbol)
    {
        // 1. If it's a Task<T>, check the inner T
        if (IsGenericTask(typeSymbol, out ITypeSymbol innerType))
        {
            // Return true if the inner type is nullable OR if the Task itself is nullable (Task<T>?)
            return IsTypeDirectlyNullable(innerType) || IsTypeDirectlyNullable(typeSymbol);
        }

        // 2. Check the type itself
        return IsTypeDirectlyNullable(typeSymbol);
    }

    /// <summary>
    /// Returns a non-nullable version of the type. 
    /// If the type is a Task<T?>, it reconstructs and returns Task<T>.
    /// </summary>
    public static ITypeSymbol MakeNonNullable(ITypeSymbol typeSymbol)
    {
        // 1. If it's a Task<T>, reconstruct it with a non-nullable T
        if (IsGenericTask(typeSymbol, out ITypeSymbol innerType))
        {
            // Make the inner type non-nullable
            ITypeSymbol nonNullableInner = MakeTypeDirectlyNonNullable(innerType);

            // Get the original unbound generic definition (Task<>)
            INamedTypeSymbol namedType = (INamedTypeSymbol)typeSymbol;
            INamedTypeSymbol unboundTaskType = namedType.OriginalDefinition;

            // Reconstruct the Task with the new non-nullable inner type -> Task<T>
            ITypeSymbol reconstructedTask = unboundTaskType.Construct(nonNullableInner);

            // Ensure the Task itself isn't nullable (Task?) and return it
            return MakeTypeDirectlyNonNullable(reconstructedTask);
        }

        // 2. Make the type itself non-nullable
        return MakeTypeDirectlyNonNullable(typeSymbol);
    }

    // --- Private Internal Helpers ---

    private static bool IsGenericTask(ITypeSymbol typeSymbol, out ITypeSymbol innerType)
    {
        if (typeSymbol is INamedTypeSymbol namedType && namedType.IsGenericType)
        {
            string ns = namedType.ContainingNamespace?.ToDisplayString() ?? "";
            string name = namedType.MetadataName; // MetadataName includes the tick, e.g. "Task`1"

            if (ns == "System.Threading.Tasks" && (name == "Task`1" || name == "ValueTask`1"))
            {
                innerType = namedType.TypeArguments[0];
                return true;
            }
        }

        innerType = null!;
        return false;
    }

    private static bool IsTypeDirectlyNullable(ITypeSymbol type)
    {
        // Check Value Types (int?)
        if (type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T) return true;
        
        // Check Reference Types (string?)
        if (type.NullableAnnotation == NullableAnnotation.Annotated) return true;

        return false;
    }

    private static ITypeSymbol MakeTypeDirectlyNonNullable(ITypeSymbol type)
    {
        // Strip Value Type Nullability (Nullable<T> -> T)
        if (type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T && type is INamedTypeSymbol namedType)
        {
            return namedType.TypeArguments[0];
        }

        // Strip Reference Type Nullability (string? -> string)
        return type.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
    }
}
