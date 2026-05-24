using System.ComponentModel;
using PamelloV7.Framework.Core.PEQL;
using PamelloV7.Framework.Shared.Entities.Base;
using PamelloV7.Framework.Shared.Variants.Attributes;

namespace PamelloV7.Framework.Core.Actions;

public static partial class PamelloStaticActions
{
    private static Type GetTypeGeneric<TConvertToType>() => typeof(TConvertToType);
    
    public static async Task<object?> ConvertStringAsync(
        [Variant(nameof(GetTypeGeneric))]
        Type type,
        string str,
        string defaultQuery = "",
        IPamelloEntityQueryService? peql = null
    ) {
        var query = string.IsNullOrEmpty(str) ? defaultQuery : str;

        if (type.IsAssignableTo(typeof(IPamelloBasicEntity))) {
            if (peql is null) return null;
            return await peql.GetSingleAsync(type, query);
        }
        if (
            type.IsGenericType && (
                type.GetGenericTypeDefinition() == typeof(List<>) ||
                type.GetGenericTypeDefinition() == typeof(IEnumerable<>)) &&
            type.GenericTypeArguments.First().IsAssignableTo(typeof(IPamelloBasicEntity))
        ) {
            if (peql is null) return null;
            return await peql.GetAsync(type.GenericTypeArguments.First(), query);
        }
        
        var converter = TypeDescriptor.GetConverter(type);
        return converter.ConvertFromString(str);
    }
    
}
