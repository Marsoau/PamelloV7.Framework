using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.App;
using PamelloV7.Framework.Core.Data;
using PamelloV7.Framework.Core.Exceptions;
using PamelloV7.Framework.Shared.Exceptions;

namespace PamelloV7.Framework.Data;

public class DatabaseAccessService : IDatabaseAccessService
{
    private readonly LiteDatabase _db;
    
    public DatabaseAccessService(IServiceProvider services) {
        var options = services.GetRequiredService<PamelloAppOptions>();
        
        var file = new FileInfo(Path.Combine(options.DataPath, "lite.db"));
        if (!file.Directory?.Exists ?? false) file.Directory.Create();
        
        _db = new LiteDatabase(file.FullName);
    }

    public IDatabaseCollection<TType> GetCollection<TType>(string name) {
        if (typeof(TType).GetProperty("Id") is null) {
            throw new PamelloDatabaseException("Collection type must have property named \"Id\"");
        }
        
        var collection = _db.GetCollection<TType>(name);

        return new DatabaseCollection<TType>(collection);
    }
}
