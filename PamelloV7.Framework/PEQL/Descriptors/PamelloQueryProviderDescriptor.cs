using PamelloV7.Framework.Core.Repositories;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.PEQL.Descriptors;

public class PamelloQueryProviderDescriptor(
    string name,
    Type entityType,
    IPamelloRepository repository,
    List<PamelloQueryProviderPointDescriptor> points
)
{
    public readonly string Name = name;
    public readonly Type EntityType = entityType;
    public readonly IPamelloRepository Repository = repository;
    public readonly List<PamelloQueryProviderPointDescriptor> Points = points;

    public TPamelloEntity? GetSingleById<TPamelloEntity>(int id)
        where TPamelloEntity : class, IPamelloBasicEntity
    {
        return Repository.Get<TPamelloEntity>(id);
    }
    
    public IEnumerable<TPamelloEntity> GetByIds<TPamelloEntity>(params int[] ids)
        where TPamelloEntity : class, IPamelloBasicEntity
    {
        return ids.Select(id => Repository.Get<TPamelloEntity>(id)).OfType<TPamelloEntity>();
    }

    public PamelloQueryProviderPointDescriptor? GetPointByName(string name) {
        return Points.FirstOrDefault(p => p.Attribute.Names.Contains(name));
    }
};
