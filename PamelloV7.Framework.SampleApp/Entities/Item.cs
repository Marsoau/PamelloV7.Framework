using PamelloV7.Framework.Core.Entities;
using PamelloV7.Framework.Core.Entities.Attributes;
using PamelloV7.Framework.Shared.Attributes;
using PamelloV7.Framework.Shared.Entities.Dto;

namespace PamelloV7.Framework.SampleApp.Entities;

[PamelloBasicEntity]
public partial class Item
{
    private static int _idCounter;
    public override int Id { get; } = ++_idCounter;
    
    public int Number { get; private set; }
    
    public Item(int number) {
        Number = number;
    }

    public override string ToString() {
        return $"[{Id}] {Number}";
    }

    public override PamelloBasicEntityDto GetDto() {
        throw new NotImplementedException();
    }
}
