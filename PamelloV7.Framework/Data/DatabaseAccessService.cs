using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.App;
using PamelloV7.Framework.Core.Data;
using PamelloV7.Framework.Core.Entities.Dao;
using PamelloV7.Framework.Core.Exceptions;
using PamelloV7.Framework.Core.Repositories;
using PamelloV7.Framework.Repositories.Loaders;
using PamelloV7.Framework.Shared.Exceptions;

namespace PamelloV7.Framework.Data;

public class DatabaseAccessService : IDatabaseAccessService
{
    private readonly IServiceProvider _services;
    
    private readonly PamelloRepositoriesLoader _repositoriesLoader;
        
    private readonly LiteDatabase _db;
    
    public DatabaseAccessService(IServiceProvider services) {
        _services = services;
        
        _repositoriesLoader = services.GetRequiredService<PamelloRepositoriesLoader>();
        
        var options = services.GetRequiredService<PamelloAppOptions>();
        
        var file = new FileInfo(Path.Combine(options.DataPath, "lite.db"));
        if (!file.Directory?.Exists ?? false) file.Directory?.Create();
        
        _db = new LiteDatabase(file.FullName);
    }

    public IDatabaseCollection<TType> GetCollection<TType>(string name) {
        if (typeof(TType).GetProperty("Id") is null) {
            throw new PamelloDatabaseException("Collection type must have property named \"Id\"");
        }
        
        var collection = _db.GetCollection<TType>(name);

        return new DatabaseCollection<TType>(collection);
    }
    
    public IDatabaseCollection<PamelloBasicDao>? GetCollectionOfEntity(Type entityType) {
        var repositoryDescriptor = _repositoriesLoader.RepositoriesDescriptors
            .FirstOrDefault(repo =>
                repo.Attribute.EntityType == entityType && repo.IsDatabaseRepository
            );

        if (repositoryDescriptor is null || _services.GetService(repositoryDescriptor.RepositoryType)
            is not IPamelloDatabaseRepository databaseRepository
        ) return null;
        
        return databaseRepository.GetCollection();
    }
}
