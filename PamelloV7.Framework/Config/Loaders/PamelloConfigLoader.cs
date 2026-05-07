using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using PamelloV7.Framework.App;
using PamelloV7.Framework.Config.Parts;
using PamelloV7.Framework.Core.Config;
using PamelloV7.Framework.Core.Config.Attributes;
using PamelloV7.Framework.Core.Config.Loaders;
using PamelloV7.Framework.Core.Config.Parts;
using PamelloV7.Framework.Core.Logging;

namespace PamelloV7.Framework.Config.Loaders;

public class PamelloConfigLoader : IPamelloConfigLoader
{
    private readonly PamelloAppOptions _options;
    
    private JsonObject? _json;
    public JsonObject Json => _json ?? throw new InvalidOperationException("Json not set");
    
    public List<IPamelloConfigPart> Parts { get; private set; }

    private readonly JsonSerializerOptions _jsoncProperties;

    public PamelloConfigLoader(PamelloAppOptions options) {
        _options = options;
        
        Parts = [];
        
        _jsoncProperties = new JsonSerializerOptions {
            ReadCommentHandling = JsonCommentHandling.Skip
        };
    }

    public void Load() {
        var configFile = new FileInfo(Path.Combine(_options.ConfigPath, "config.jsonc"));
        
        if (!(configFile.Directory?.Exists ?? true)) configFile.Directory.Create();

        if (!configFile.Exists) {
            configFile.Create().Close();
        }

        try {
            using var fs = configFile.OpenRead();
            _json = JsonSerializer.Deserialize<JsonObject>(fs, _jsoncProperties);
        }
        catch {
            _json = new JsonObject();
        }

        foreach (var (partName, partJson) in Json) {
            if (partJson is null) continue;
            
            Parts.Add(new PamelloConfigPart(partName, partJson));
        }
    }

    public void FinishBeforeModules() {
        var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes()).ToList();
        var rootNodes = types.Where(t =>
            t is { IsClass: true, IsAbstract: false } &&
            t.GetCustomAttribute<ConfigRootAttribute>() is not null
        );
        
        var configTypes = new Dictionary<string, KeyValuePair<Type, Type>>();
        
        foreach (var rootNode in rootNodes) {
            var attribute = rootNode.GetCustomAttribute<ConfigRootAttribute>()!;
            
            var staticType = types.FirstOrDefault(t => t.Name == rootNode.Name.Replace("Node", "Config"));
            var nodeType = types.FirstOrDefault(t => t.Name == rootNode.Name);
        
            if (staticType is null || nodeType is null) return;
            
            attribute.Name ??= rootNode.Name.Replace("Node", "");
        
            configTypes.Add(attribute.Name.Split(":").LastOrDefault() ?? "", new KeyValuePair<Type, Type>(staticType, nodeType));
        }

        foreach (var (partName, (staticType, nodeType)) in configTypes) {
            var part = Parts.FirstOrDefault(x => x.Name == partName);
            if (part is null) {
                part = new PamelloConfigPart(partName, new JsonObject(), true);
                Parts.Add(part);
            }
        
            part.Initialize(nodeType, staticType, null);
            part.Finish();
        }
    }
    
    /*
    public void InitializeFromContainer(PamelloModuleContainer container) {
        foreach (var (partName, staticAndNodeType) in container.ConfigTypes) {
            var fullName = $"{container.Module.Author}/{container.Module.Name}{
                (!string.IsNullOrWhiteSpace(partName) ? $":{partName}" : "")
            }";
            var (partStaticType, partNodeType) = staticAndNodeType;
            
            var part = Parts.FirstOrDefault(part => part.Name == fullName);
            if (part is null) {
                part = new PamelloConfigPart(fullName, new JsonObject(), true);
                Parts.Add(part);
            }
            
            part.Initialize(partNodeType, partStaticType, container.Module);
        }
    }

    public void FinishFromContainer(PamelloModuleContainer container) {
        Parts.Where(part => part.Module == container.Module).ToList().ForEach(part => part.Finish());
    }
    */

    /*
    public void InitType(Type type, string partName) {
        var rootProperty = type.GetField("Root");
        if (rootProperty is null) throw new PamelloLoadingException($"Root property not found in config type {type.FullName}");

        var container = Parts.FirstOrDefault(x => x.Name == partName);
        if (container is null) throw new PamelloLoadingException($"Config part \"{partName}\" not found in config file");
        if (container.Json.ValueKind == JsonValueKind.Null) throw new PamelloLoadingException($"Config part \"{container.Name}\" not found in config file");

        foreach (var property in rootProperty.FieldType.GetProperties()) {
            Output.Write($"| {property.Name}: {property.PropertyType}", ELogLevel.Debug);
        }

        rootProperty.SetValue(null, container.Json.Deserialize(rootProperty.FieldType, _jsoncProperties));
    }
    */
}
