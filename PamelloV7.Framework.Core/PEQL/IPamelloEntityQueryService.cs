using System.Reflection;
using PamelloV7.Framework.Core.Exceptions;
using PamelloV7.Framework.Core.Repositories;
using PamelloV7.Framework.Core.Services.Base;
using PamelloV7.Framework.Shared.Entities.Base;
using PamelloV7.Framework.Shared.Exceptions;

namespace PamelloV7.Framework.Core.PEQL;

public interface IPamelloEntityQueryService : IPamelloService
{
    public Task<IEnumerable<TPamelloEntity>> GetAsync<TPamelloEntity>(string query);
    
    public TPamelloEntity? GetSingleById<TPamelloEntity>(int id)
        where TPamelloEntity : class, IPamelloBasicEntity;
    public IEnumerable<TPamelloEntity> GetByIds<TPamelloEntity>(params int[] ids);
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
    
    public static Task<IPamelloBasicEntity> GetSingleRequiredAsync(this IPamelloEntityQueryService repository, string query) => throw new NotImplementedException();
    public static Task<IPamelloBasicEntity> GetSingleRequiredAsync(this IPamelloEntityQueryService repository, Type entityType, string query) => throw new NotImplementedException();
    public static Task<TPamelloEntity> GetSingleRequiredAsync<TPamelloEntity>(this IPamelloEntityQueryService repository, string query) => throw new NotImplementedException();

    public static IPamelloBasicEntity GetSingleByIdRequired(this IPamelloEntityQueryService repository, Type entityType, int id)
        => (IPamelloBasicEntity)RunGenericMethod(typeof(PamelloEntityQueryServiceExtensions), null, nameof(GetSingleByIdRequired), [entityType], [repository, id])!;
    public static TPamelloEntity GetSingleByIdRequired<TPamelloEntity>(this IPamelloEntityQueryService repository, int id)
        where TPamelloEntity : class, IPamelloBasicEntity
        => repository.GetSingleById<TPamelloEntity>(id) ?? throw NotFoundByIdException(typeof(TPamelloEntity), id);
    
    public static Task<IPamelloBasicEntity> GetSingleAsync(this IPamelloEntityQueryService repository, string query) => throw new NotImplementedException();
    public static Task<IPamelloBasicEntity> GetSingleAsync(this IPamelloEntityQueryService repository, Type entityType, string query) => throw new NotImplementedException();
    public static Task<TPamelloEntity> GetSingleAsync<TPamelloEntity>(this IPamelloEntityQueryService repository, string query) => throw new NotImplementedException();
    
    public static Task<IEnumerable<IPamelloBasicEntity>> GetAsync(this IPamelloEntityQueryService repository, string query) => throw new NotImplementedException();
    public static Task<IEnumerable<IPamelloBasicEntity>> GetAsync(this IPamelloEntityQueryService repository, Type entityType, string query) => throw new NotImplementedException();
    
    public static IPamelloBasicEntity? GetSingleById(this IPamelloEntityQueryService queries, Type entityType, int id)
        => RunGenericMethod(typeof(IPamelloEntityQueryService), queries, nameof(IPamelloEntityQueryService.GetSingleById), [entityType], [id]) as IPamelloBasicEntity;

    public static IEnumerable<IPamelloBasicEntity> GetByIds(this IPamelloEntityQueryService repository, Type entityType, params int[] ids) => throw new NotImplementedException();
}