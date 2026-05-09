namespace PamelloV7.Framework.Shared.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class AutoInheritAttribute : Attribute
{
    public Type? ClassType { get; }
    public Type[]? InterfaceTypes { get; }
    
    public AutoInheritAttribute(Type? classType, Type[] interfaceTypes) {
        ClassType = classType;
        InterfaceTypes = interfaceTypes;
    }
}
