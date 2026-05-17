using PamelloV7.Framework.Core.Services.Base;
using PamelloV7.Framework.Shared.Entities.Base;

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
    public static Task<IPamelloBasicEntity> GetSingleRequiredAsync(string query) => throw new NotImplementedException();
    public static Task<IPamelloBasicEntity> GetSingleRequiredAsync(Type entityType, string query) => throw new NotImplementedException();
    public static Task<TPamelloEntity> GetSingleRequiredAsync<TPamelloEntity>(string query) => throw new NotImplementedException();
    
    public static Task<IPamelloBasicEntity> GetSingleAsync(string query) => throw new NotImplementedException();
    public static Task<IPamelloBasicEntity> GetSingleAsync(Type entityType, string query) => throw new NotImplementedException();
    public static Task<TPamelloEntity> GetSingleAsync<TPamelloEntity>(string query) => throw new NotImplementedException();
    
    public static Task<IEnumerable<IPamelloBasicEntity>> GetAsync(string query) => throw new NotImplementedException();
    public static Task<IEnumerable<IPamelloBasicEntity>> GetAsync(Type entityType, string query) => throw new NotImplementedException();
    
    public static IPamelloBasicEntity GetSingleById(Type entityType, int id) => throw new NotImplementedException();

    public static IEnumerable<IPamelloBasicEntity> GetByIds(Type entityType, params int[] ids) => throw new NotImplementedException();
}