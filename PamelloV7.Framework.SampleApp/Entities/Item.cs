using PamelloV7.Framework.Core.Entities;
using PamelloV7.Framework.Core.Entities.Attributes;
using PamelloV7.Framework.Core.Scope;
using PamelloV7.Framework.Shared.Attributes;
using PamelloV7.Framework.Shared.Entities.Dto;

namespace PamelloV7.Framework.SampleApp.Entities;

[PamelloBasicDatabaseEntity]
public partial class Item
{
    public partial int Number { get; set; }
    public partial string Message { get; set; }

    public Item(int number, string message) {
        _number = number;
        _message = message;
    }

    public override bool IsAvailable() {
        return base.IsAvailable();
        //&& PamelloAppScope.User?.Id == 1;
    }

    public override string ToString()
        => $"[{Id}] {Number} \"{Message}\"";
}
