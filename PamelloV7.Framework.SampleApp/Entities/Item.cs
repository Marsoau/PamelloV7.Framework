using PamelloV7.Framework.Core.Entities;

namespace PamelloV7.Framework.SampleApp.Entities;

public class Item : PamelloBasicEntity
{
    public int Number { get; set; }
    
    private static int _idCounter = 0;
    public Item(int number) {
        Id = ++_idCounter;
        
        Number = number;
    }

    public override string ToString() {
        return $"[{Id}] {Number}";
    }
}
