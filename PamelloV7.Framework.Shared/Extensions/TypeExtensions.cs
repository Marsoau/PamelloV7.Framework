namespace PamelloV7.Framework.Shared.Extensions;

public static class TypeExtensions
{
    public static IEnumerable<Type> GetAllBaseTypes(this Type type, Predicate<Type>? predicate = null) {
        predicate ??= _ => true;
        
        var baseType = type.BaseType;
        while (baseType is not null && baseType != typeof(object)) {
            if (!predicate(baseType)) break;

            yield return baseType;
            baseType = baseType.BaseType;
        }
    }
}
