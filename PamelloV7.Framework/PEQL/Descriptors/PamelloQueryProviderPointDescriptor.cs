using System.Collections;
using System.Reflection;
using PamelloV7.Framework.Core.Actions;
using PamelloV7.Framework.Core.PEQL;
using PamelloV7.Framework.Core.PEQL.Attributes;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.PEQL.Descriptors;

public class PamelloQueryProviderPointDescriptor(
    ProviderPointAttribute attribute,
    MethodInfo targetMethod,
    object? providerInstance
)
{
    public readonly MethodInfo TargetMethod = targetMethod;
    public readonly ProviderPointAttribute Attribute = attribute;
    public readonly object? ProviderInstance = providerInstance;

    public async IAsyncEnumerable<TPamelloEntity> Execute<TPamelloEntity>(string argumentsString, IPamelloEntityQueryService? peql)
        where TPamelloEntity : class, IPamelloBasicEntity
    {
        var arguments = await PamelloStaticActions.EnumerateArgumentsForParameters(
            argumentsString,
            TargetMethod.GetParameters(),
            peql
        ).ToArrayAsync();
        
        var result = TargetMethod.Invoke(ProviderInstance, arguments);
        if (result is null) yield break;
        
        var resultType = result.GetType();
        if (resultType.IsAssignableTo(typeof(TPamelloEntity))) {
            yield return (TPamelloEntity)result;
        }
        else if (resultType.GetGenericTypeDefinition() == typeof(Task<>)
            || resultType.GetGenericTypeDefinition() == typeof(ValueTask<>)
        ) {
            yield return (TPamelloEntity) await (dynamic)result;
        }
        else if (resultType.IsAssignableTo(typeof(IEnumerable))) {
            foreach (var entity in ((IEnumerable)result).OfType<TPamelloEntity>()) {
                yield return entity;
            }
        }
        else if (resultType.IsAssignableTo(typeof(IAsyncEnumerable<TPamelloEntity>))) {
            await foreach (var entity in (IAsyncEnumerable<TPamelloEntity>)result) {
                yield return entity;
            }
        }
    }
}
