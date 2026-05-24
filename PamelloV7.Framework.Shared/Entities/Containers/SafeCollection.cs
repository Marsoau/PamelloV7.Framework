using System.Collections;
using System.Text.Json;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.Shared.Entities.Containers;

public class SafeCollection<TPamelloEntity> : ISafeCollection, ICollection<TPamelloEntity>
    where TPamelloEntity : class, IPamelloBasicEntity
{
    private List<Safe<TPamelloEntity>> _entities;
    
    public IEnumerable<int> Ids => _entities.Select(e => e.Id);
    public IEnumerable<TPamelloEntity?> Entities => _entities.Select(e => e.Entity);
    public IEnumerable<TPamelloEntity> AvailableEntities => Entities.OfType<TPamelloEntity>();
    
    IEnumerable<IPamelloBasicEntity?> ISafeCollection.Entities => Entities;
    IEnumerable<IPamelloBasicEntity> ISafeCollection.AvailableEntities => AvailableEntities;
    
    public Type EntityType => typeof(TPamelloEntity);

    public SafeCollection() {
        _entities = [];
    }
    public SafeCollection(IEnumerable<TPamelloEntity> entities) {
        _entities = entities.Select(e => new Safe<TPamelloEntity>(e.Id)).ToList();
    }
    public SafeCollection(IEnumerable<int> ids) {
        _entities = ids.Select(id => new Safe<TPamelloEntity>(id)).ToList();
    }
    
    public IEnumerator<TPamelloEntity> GetEnumerator() {
        return AvailableEntities.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    public void Add(TPamelloEntity item) {
        _entities.Add(new Safe<TPamelloEntity>(item.Id));
    }
    public void Clear() {
        _entities.Clear();
    }
    public bool Contains(TPamelloEntity item) {
        return _entities.Any(e => e.Id == item.Id);
    }
    public void CopyTo(TPamelloEntity[] array, int arrayIndex) {
        AvailableEntities.ToList().CopyTo(array, arrayIndex);
    }
    public bool Remove(TPamelloEntity item) {
        if (_entities.FirstOrDefault(e => e.Id == item.Id) is not { } entity) return false;
        
        _entities.Remove(entity);
        return true;
    }
    
    public int Count => _entities.Count;
    public bool IsReadOnly => false;
}
