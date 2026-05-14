using PamelloV7.Framework.Core.Entities;
using PamelloV7.Framework.Core.Entities.Attributes;
using PamelloV7.Framework.Shared.Attributes;
using PamelloV7.Framework.Shared.Entities.Dto;

namespace PamelloV7.Framework.SampleApp.Entities;

[PamelloBasicDatabaseEntity]
public partial class Item
{
    public partial int Number { get; set; }
    public partial string Message { get; set; }

    public Item(int number, string message) {
        Number = number;
        Message = message;
    }

    public override string ToString() {
        return $"[{Id}] {Number} \"{Message}\"";
    }
}
