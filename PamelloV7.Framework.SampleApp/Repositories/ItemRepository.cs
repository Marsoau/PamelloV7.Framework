using PamelloV7.Framework.Core.Entities;
using PamelloV7.Framework.Core.Repositories;
using PamelloV7.Framework.SampleApp.Entities;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.SampleApp.Repositories;

public interface IItemRepository : IPamelloRepository<Item>
{
    public IEnumerable<Item> GetBySomeNumber(int number);

    public Item Add(int number);
}

public class ItemRepository : PamelloRepository<Item>, IItemRepository
{
    public ItemRepository(IServiceProvider services) : base(services) { }

    public IEnumerable<Item> GetBySomeNumber(int number) {
        return Available.Where(e => e.Number == number);
    }

    public Item Add(int number) => LoadPamelloEntity(new Item(number));
}
