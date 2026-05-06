namespace PamelloV7.Framework.Core.Config.Attributes;

public class ConfigRootAttribute : Attribute
{
    public string? Name { get; set; }

    public ConfigRootAttribute() { } 
    public ConfigRootAttribute(string name) {
        Name = name;
    }
}
