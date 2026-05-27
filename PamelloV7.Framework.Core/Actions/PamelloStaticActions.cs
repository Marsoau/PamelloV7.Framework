using System.ComponentModel;
using System.Reflection;
using PamelloV7.Framework.Core.PEQL;
using PamelloV7.Framework.Core.PEQL.Blocks;
using PamelloV7.Framework.Shared.Entities.Base;
using PamelloV7.Framework.Shared.Exceptions;
using PamelloV7.Framework.Shared.Variants.Attributes;

namespace PamelloV7.Framework.Core.Actions;

public static partial class PamelloStaticActions
{
    public static async Task<object?> ConvertStringAsync(
        Type type,
        string str,
        IPamelloEntityQueryService? peql
    ) {
        if (type.IsAssignableTo(typeof(IPamelloBasicEntity))) {
            if (peql is null) return null;
            return await peql.GetSingleAsync(type, str);
        }
        if (
            type.IsGenericType && (
                type.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>)
            ) && type.GenericTypeArguments.First().IsAssignableTo(typeof(IPamelloBasicEntity))
        ) {
            //todo - decide weather to return IAsyncEnumerable here or iterate it / return a blocking one
            return peql?.GetAsync(type.GenericTypeArguments.First(), str);
        }
        
        var converter = TypeDescriptor.GetConverter(type);
        return converter.ConvertFromString(str);
    }
    
    public static async IAsyncEnumerable<object?> EnumerateArgumentsForParameters(string argumentsString, ParameterInfo[] parameters, IPamelloEntityQueryService? peql) {
        var argumentsBlocks = argumentsString.EnumerateStringBlocks([',']).ToList();
        if (argumentsBlocks.Count < parameters.Count(p => !p.HasDefaultValue)) throw new PamelloException("Not enough arguments for parameters");
        
        for (int i = 0; i < parameters.Length; i++) {
            var parameter = parameters[i];
            var argumentText = argumentsBlocks.ElementAtOrDefault(i)?.Text;
            
            if (argumentText is not null && string.IsNullOrEmpty(argumentText)) argumentText = null;

            if (argumentText is null) {
                if (parameter.HasDefaultValue) yield return parameter.DefaultValue;
                else throw new PamelloException($"Missing argument for parameter \"{parameter.Name}\"");
                
                continue;
            }

            yield return await ConvertStringAsync(parameter.ParameterType, argumentText, peql);
        }
    }
}
