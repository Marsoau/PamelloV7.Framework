using PamelloV7.Framework.Core.Entities;
using PamelloV7.Framework.Shared.Entities.Dto;

namespace PamelloV7.Framework.SampleApp.Entities;

public partial class Item
{
    public partial int Number {
        get; private set => field = value;
    }
}

public partial class Item : PamelloBasicEntity
{
    public partial int Number { get; private set; }
    
    private static int _idCounter;
    public Item(int number) {
        Id = ++_idCounter;
        
        Number = number;
    }

    public override string ToString() {
        return $"[{Id}] {Number}";
    }
}
