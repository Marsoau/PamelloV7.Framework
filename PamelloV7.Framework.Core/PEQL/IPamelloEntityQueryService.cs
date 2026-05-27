using System.Reflection;
using PamelloV7.Framework.Core.Exceptions;
using PamelloV7.Framework.Core.Repositories;
using PamelloV7.Framework.Core.Services.Base;
using PamelloV7.Framework.Shared.Entities.Base;
using PamelloV7.Framework.Shared.Exceptions;

namespace PamelloV7.Framework.Core.PEQL;

public interface IPamelloEntityQueryService : IPamelloService
{
    public IAsyncEnumerable<TPamelloEntity> GetAsync<TPamelloEntity>(string query)
        where TPamelloEntity : class, IPamelloBasicEntity;

    public TPamelloEntity? GetSingleById<TPamelloEntity>(int id)
        where TPamelloEntity : class, IPamelloBasicEntity;
    public IEnumerable<TPamelloEntity> GetByIds<TPamelloEntity>(params int[] ids)
        where TPamelloEntity : class, IPamelloBasicEntity;
}

public static class PamelloEntityQueryServiceExtensions
{
    private static PamelloException NotFoundByIdException(Type entityType, int id)
        => new PamelloDatabaseException($"Entity of type \"{entityType}\" with id {id} not found");

    private static object? RunGenericMethod(Type typeMethodIsOn, object? objectMethodIsOn, string methodName, Type[] genericTypes, object?[] args) {
        var method = typeMethodIsOn.GetMethods()
            .FirstOrDefault(method =>
                method.IsGenericMethod &&
                method.Name == methodName &&
                method.GetGenericArguments().Length == genericTypes.Length
            );
        if (method is null) throw new Exception($"Method {methodName} not found on type {typeMethodIsOn.FullName}");

        return method.MakeGenericMethod(genericTypes).Invoke(objectMethodIsOn, args);
    }

    public static async ValueTask<IPamelloBasicEntity> GetSingleRequiredAsync(this IPamelloEntityQueryService repository, string query)
        => await repository.GetSingleAsync(query) ?? throw new PamelloException($"Not a single entity was found by \"{query}\"");
    public static async ValueTask<IPamelloBasicEntity> GetSingleRequiredAsync(this IPamelloEntityQueryService repository, Type entityType, string query)
        => await repository.GetSingleAsync(entityType, query) ?? throw new PamelloException($"Not a single entity of type \"{entityType.Name}\" was found by query \"{query}\"");
    public static async ValueTask<TPamelloEntity> GetSingleRequiredAsync<TPamelloEntity>(this IPamelloEntityQueryService repository, string query)
        where TPamelloEntity : class, IPamelloBasicEntity
        => await repository.GetSingleAsync<TPamelloEntity>(query) ?? throw new PamelloException($"Not a single entity of type \"{typeof(TPamelloEntity).Name}\" was found by query \"{query}\"");

    public static IPamelloBasicEntity GetSingleByIdRequired(this IPamelloEntityQueryService repository, Type entityType, int id)
        => (IPamelloBasicEntity)RunGenericMethod(typeof(PamelloEntityQueryServiceExtensions), null, nameof(GetSingleByIdRequired), [entityType], [repository, id])!;
    public static TPamelloEntity GetSingleByIdRequired<TPamelloEntity>(this IPamelloEntityQueryService repository, int id)
        where TPamelloEntity : class, IPamelloBasicEntity
        => repository.GetSingleById<TPamelloEntity>(id) ?? throw NotFoundByIdException(typeof(TPamelloEntity), id);

    public static ValueTask<IPamelloBasicEntity?> GetSingleAsync(this IPamelloEntityQueryService repository, string query) => repository.GetAsync(query).FirstOrDefaultAsync();
    public static ValueTask<IPamelloBasicEntity?> GetSingleAsync(this IPamelloEntityQueryService repository, Type entityType, string query) => repository.GetAsync(entityType, query).FirstOrDefaultAsync();
    public static ValueTask<TPamelloEntity?> GetSingleAsync<TPamelloEntity>(this IPamelloEntityQueryService repository, string query)
        where TPamelloEntity : class, IPamelloBasicEntity
        => repository.GetAsync<TPamelloEntity>(query).FirstOrDefaultAsync();

    public static IAsyncEnumerable<IPamelloBasicEntity> GetAsync(this IPamelloEntityQueryService repository, string query) => repository.GetAsync<IPamelloBasicEntity>(query);
    public static IAsyncEnumerable<IPamelloBasicEntity> GetAsync(this IPamelloEntityQueryService repository, Type entityType, string query)
        => (IAsyncEnumerable<IPamelloBasicEntity>)RunGenericMethod(typeof(IPamelloEntityQueryService), repository, nameof(IPamelloEntityQueryService.GetAsync), [entityType], [query])!;

    public static IPamelloBasicEntity? GetSingleById(this IPamelloEntityQueryService queries, Type entityType, int id)
        => RunGenericMethod(typeof(IPamelloEntityQueryService), queries, nameof(IPamelloEntityQueryService.GetSingleById), [entityType], [id]) as IPamelloBasicEntity;

    public static IEnumerable<IPamelloBasicEntity> GetByIds(this IPamelloEntityQueryService repository, Type entityType, params int[] ids)
        => (IEnumerable<IPamelloBasicEntity>)RunGenericMethod(typeof(IPamelloEntityQueryService), repository, nameof(IPamelloEntityQueryService.GetByIds), [entityType], [ids])!;
}