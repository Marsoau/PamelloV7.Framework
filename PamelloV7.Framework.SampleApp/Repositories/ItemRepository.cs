using PamelloV7.Framework.Core.Entities;
using PamelloV7.Framework.Core.Repositories;
using PamelloV7.Framework.Core.Repositories.Attributes;
using PamelloV7.Framework.SampleApp.Entities;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.SampleApp.Repositories;

public interface IItemRepository : IPamelloRepository<Item>
{
    public IEnumerable<Item> GetBySomeNumber(int number);

    public Item Add(int number, string message);
}

[PamelloDatabaseRepository<Item>("items")]
public partial class ItemRepository : IItemRepository
{
    public IEnumerable<Item> GetBySomeNumber(int number) {
        return Available.Where(e => e.Number == number);
    }

    public Item Add(int number, string message) => LoadPamelloEntity(new Item(number, message));
}
