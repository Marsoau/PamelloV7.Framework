using System.Text.Json.Nodes;
using PamelloV7.Framework.Core.Modules;

namespace PamelloV7.Framework.Core.Config.Parts;

public interface IPamelloConfigPart
{
    public string Name { get; }
    public JsonNode Json { get; }
    public Type NodeType { get; }
    public Type StaticType { get; }
    public object? Node { get; }
    public PamelloModule? Module { get; }
    
    public List<IPamelloConfigPreInitializer> PreInitializers { get; }
    
    public bool IsJustCreated { get; }
    public bool IsInitialized { get; }

    public void Initialize(Type nodeType, Type staticType, PamelloModule? module);

    public IEnumerable<IPamelloConfigPreInitializer> GetPreInitializers(Type nodeType, string? previousPath = null);

    public void Finish();
}
