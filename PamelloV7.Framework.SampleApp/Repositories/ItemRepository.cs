using PamelloV7.Framework.Core.Entities;
using PamelloV7.Framework.Core.PEQL.Attributes;
using PamelloV7.Framework.Core.Repositories;
using PamelloV7.Framework.Core.Repositories.Attributes;
using PamelloV7.Framework.SampleApp.Entities;
using PamelloV7.Framework.Shared.Entities.Base;

namespace PamelloV7.Framework.SampleApp.Repositories;

[PamelloDatabaseRepository<Item>("items")]
public partial class ItemRepository
{
    [ProviderPoint("number")]
    public IEnumerable<Item> GetBySomeNumber(int number) {
        return Available.Where(e => e.Number == number);
    }
    
    [ProviderPoint("test")]
    public IEnumerable<Item> GetTest() {
        return Available.Where(e => e.Number % 2 == 0);
    }

    public Item Add(int number, string message) => LoadPamelloEntity(new Item(number, message));
}
