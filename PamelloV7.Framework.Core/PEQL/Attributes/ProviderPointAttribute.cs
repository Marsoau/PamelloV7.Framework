namespace PamelloV7.Framework.Core.PEQL.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class ProviderPointAttribute : Attribute
{
    public string[] Names { get; set; }
    
    public ProviderPointAttribute(string name) {
        Names = [name];
    }
    public ProviderPointAttribute(string[] names) {
        Names = names;
    }

    public override string ToString() => string.Join(", ", Names);
}
