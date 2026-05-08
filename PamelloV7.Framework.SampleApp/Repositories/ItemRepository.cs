using PamelloV7.Framework.Core.Entities;
using PamelloV7.Framework.Core.Repositories;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.SampleApp.Repositories;

public class Item : PamelloBasicEntity
{
    public int SomeNumber { get; set; }
    
    private static int _idCounter = 0;
    public Item() {
        Id = ++_idCounter;
    }

    public override string ToString() {
        return $"[{Id}] {SomeNumber}";
    }
}

public class AlsoItem : Item
{
    public int AnotherNumber { get; set; }

    override public string ToString() {
        return $"{base.ToString()} : {AnotherNumber}";
    }
}

public interface IItemRepository : IPamelloRepository<Item>
{
    public IEnumerable<Item> GetBySomeNumber(int number);
    
    public TType Add<TType>(TType item)
        where TType : Item;
}

public class ItemRepository : PamelloRepository<Item>, IItemRepository
{
    public ItemRepository(IServiceProvider services) : base(services) { }

    public IEnumerable<Item> GetBySomeNumber(int number) {
        return Available.Where(e => e.SomeNumber == number);
    }

    public TType Add<TType>(TType item)
        where TType : Item
        => (TType)Load(item);
}
