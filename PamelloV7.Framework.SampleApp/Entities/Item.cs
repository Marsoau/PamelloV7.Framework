using PamelloV7.Framework.Core.Entities;
using PamelloV7.Framework.Shared.Attributes;
using PamelloV7.Framework.Shared.Entities.Dto;

namespace PamelloV7.Framework.SampleApp.Entities;

[AutoInherit(typeof(PamelloBasicEntity), [])]
public partial class Item
{
    public int Number { get; private set; }
    
    private static int _idCounter;
    public Item(int number) {
        Id = ++_idCounter;
        
        Number = number;
    }

    public override string ToString() {
        return $"[{Id}] {Number}";
    }

    public override PamelloBasicEntityDto GetDto() {
        throw new NotImplementedException();
    }
}
