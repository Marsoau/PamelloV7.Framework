using PamelloV7.Framework.Core.Entities.Attributes;
using PamelloV7.Framework.Shared.Entities;

namespace PamelloV7.Framework.SampleApp.Entities;

[PamelloBasicEntity]
public partial class User : IPamelloBasicUser
{
    public partial string Name { get; set; }

    public User(string name) {
        _name = name;
    }

    public override string ToString()
        => $"[{Id}] {Name}";
}
