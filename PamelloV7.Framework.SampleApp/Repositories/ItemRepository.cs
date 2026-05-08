using PamelloV7.Framework.Core.Repositories;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.SampleApp.Repositories;

public class Item : IPamelloBasicEntity
{
    public int Id { get; }
    
    public int SomeNumber { get; set; }
    
    private static int _idCounter = 0;
    public Item() {
        Id = ++_idCounter;
    }
}

public class AlsoItem : Item
{
    public int AnotherNumber { get; set; }
}

public class OtherItem : IPamelloBasicEntity
{
    public int Id { get; }
}

public interface IItemRepository : IPamelloRepository<Item>
{
    public TType Add<TType>(TType item)
        where TType : Item;
}

public class ItemRepository : PamelloRepository<Item>, IItemRepository
{
    public ItemRepository(IServiceProvider services) : base(services) { }
    
    public TType Add<TType>(TType item)
        where TType : Item
        => (TType)Load(item);
}
