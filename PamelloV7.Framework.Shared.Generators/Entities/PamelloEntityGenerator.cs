using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PamelloV7.Framework.Shared.Generators.Base;
using PamelloV7.Framework.Shared.Generators.Extensions;
using PamelloV7.Framework.Shared.Generators.Helpers;

namespace PamelloV7.Framework.Shared.Generators.Entities;

[Generator]
public class PamelloEntityGenerator : PamelloGenerator<PamelloEntityDescriptor>
{
    public const string PamelloBasicEntityAttributeName = "PamelloBasicEntityAttribute";
    public const string PamelloBasicDatabaseEntityAttributeName = "PamelloBasicDatabaseEntityAttribute";
    
    public const string PamelloDtoClassFullName = "global::PamelloV7.Framework.Shared.Entities.Dto.PamelloBasicDto";
    public const string PamelloDboClassFullName = "global::PamelloV7.Framework.Core.Entities.Dbo.PamelloBasicDbo";
    
    protected override bool Predicate(SyntaxNode node, CancellationToken cancellationToken)
        => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };

    protected override PamelloEntityDescriptor? GetDescriptorInternal(
        GeneratorSyntaxContext context,
        INamedTypeSymbol targetType,
        StringBuilder debugOutput
    ) {
        var attribute = targetType.GetAttributes()
            .FirstOrDefault(a => SharedHelper.CheckTypeName(a.AttributeClass, PamelloBasicEntityAttributeName));

        var isDatabaseEntity = SharedHelper.CheckTypeName(attribute?.AttributeClass, PamelloBasicDatabaseEntityAttributeName);
        
        debugOutput.AppendLine($"Is database entity: {isDatabaseEntity}");
        
        if (attribute is null) return null;
        
        var hasIdProperty = targetType.GetMembers().OfType<IPropertySymbol>().Any(p => p.Name == "Id");

        var updatableProperties = new List<PamelloEntityUpdatablePropertyDescriptor>();

        foreach (var property in targetType.GetMembers().OfType<IPropertySymbol>()) {
            if (property.Name == "Id") {
                hasIdProperty = true;
                continue;
            }
            
            if (property.IsPartialDefinition) updatableProperties.Add(new PamelloEntityUpdatablePropertyDescriptor(
                property
            ));
        }
        
        return new PamelloEntityDescriptor(
            targetType,
            !hasIdProperty,
            isDatabaseEntity,
            updatableProperties,
            debugOutput
        );
    }

    public static string WriteUpdatablePropertyGetter(PamelloEntityUpdatablePropertyDescriptor propertyDescriptor) {
        if (propertyDescriptor.Property.GetMethod is null) return "//no getter";
        
        return
            $"""
            {(propertyDescriptor.Property.GetMethod.DeclaredAccessibility < propertyDescriptor.Property.DeclaredAccessibility
                ? SharedHelper.GetSymbolModifiers(propertyDescriptor.Property.GetMethod)
                : ""
            )} get => {ToPrivateFieldName(propertyDescriptor.Property.Name)};
            """;
    }
    
    public static string WriteUpdatablePropertySetter(PamelloEntityUpdatablePropertyDescriptor propertyDescriptor) {
        if (propertyDescriptor.Property.SetMethod is null) return "//no setter";
        
        return
            $"""
            {(propertyDescriptor.Property.SetMethod.DeclaredAccessibility < propertyDescriptor.Property.DeclaredAccessibility
                ? SharedHelper.GetSymbolModifiers(propertyDescriptor.Property.SetMethod)
                : ""
            )} set => {ToPrivateFieldName(propertyDescriptor.Property.Name)} = value;
            """;
    }
    
    public static string ToPrivateFieldName(string propertyName) => "_" + char.ToLower(propertyName[0]) + propertyName.Substring(1);

    private static string GetUpdatablePropertiesSource(PamelloEntityDescriptor descriptor) {
        var updatablePropertiesSb = new StringBuilder();

        foreach (var propertyDescriptor in descriptor.UpdatableProperties) {
            updatablePropertiesSb.AppendLine(
                $$"""
                protected {{propertyDescriptor.Property.Type.GetFullName()}} {{
                    ToPrivateFieldName(propertyDescriptor.Property.Name)
                }};
                {{
                    SharedHelper.GetSymbolModifiers(propertyDescriptor.Property)
                }} partial {{
                    propertyDescriptor.Property.Type.GetFullName()
                }} {{
                    propertyDescriptor.Property.Name
                }} {
                    {{WriteUpdatablePropertyGetter(propertyDescriptor)}}
                    {{WriteUpdatablePropertySetter(propertyDescriptor)}}
                }
                
                """
            );
        }
        
        return updatablePropertiesSb.ToString();
    }

    public static string GetEntityDtoAndDboClassesSource(PamelloEntityDescriptor descriptor) {
        var dtoClassSb = new StringBuilder();
        var dboClassSb = new StringBuilder();
        
        dtoClassSb.AppendLine($"public partial class Dto : {PamelloDtoClassFullName} {{");
        dboClassSb.AppendLine($"public partial class Dbo : {PamelloDboClassFullName} {{");

        foreach (var propertyDescriptor in descriptor.UpdatableProperties) {
            var line = $"{SharedHelper.Tab(1)}public required {
                propertyDescriptor.Property.Type.GetFullName()
            } {
                propertyDescriptor.Property.Name
            } {{ get; set; }}";
            
            dtoClassSb.AppendLine(line);
            dboClassSb.AppendLine(line);
        }
        
        dtoClassSb.AppendLine("}");
        dboClassSb.AppendLine("}");
        
        if (!descriptor.IsDatabaseEntity) {
            dboClassSb.Clear();
            dboClassSb.AppendLine($"//no dbo class required");
        }
        
        return $"{dtoClassSb}\n{dboClassSb}";
    }

    public static string GetEntityDtoAndDboGetter(PamelloEntityDescriptor descriptor) {
        var dtoGetterSb = new StringBuilder();
        var dboGetterSb = new StringBuilder();

        dtoGetterSb.AppendLine($"public override Dto GetDto() => new Dto() {{");
        dboGetterSb.AppendLine($"public override Dbo GetDbo() => new Dbo() {{");
        
        dtoGetterSb.AppendLine(string.Join(",\n", descriptor.UpdatableProperties
            .Select(p => p.Property.Name)
            .Prepend("Id")
            .Select(name => 
                $"{SharedHelper.Tab(1)}{name} = {name}"
            )
        ));
        dboGetterSb.AppendLine(string.Join(",\n", descriptor.UpdatableProperties
            .Select(p => p.Property.Name)
            .Prepend("Id")
            .Select(name => 
                $"{SharedHelper.Tab(1)}{name} = {ToPrivateFieldName(name)}"
            )
        ));
        
        dtoGetterSb.AppendLine("};");
        dboGetterSb.AppendLine("};");

        if (!descriptor.IsDatabaseEntity) {
            dboGetterSb.Clear();
            dboGetterSb.AppendLine($"//no dbo getter required");
        }
        
        return $"{dtoGetterSb}\n{dboGetterSb}";
    }

    public static string GetEntityDboConstructor(PamelloEntityDescriptor descriptor) {
        if (!descriptor.IsDatabaseEntity) return "//no dbo constructor required";
        var sb = new StringBuilder();
        
        sb.AppendLine($"public {descriptor.InvokingType.Name}(Dbo dbo) : base() {{");
        sb.AppendLine(string.Join("\n", descriptor.UpdatableProperties
            .Select(p => p.Property.Name)
            .Prepend("Id")
            .Select(name => 
                $"{SharedHelper.Tab(1)}{ToPrivateFieldName(name)} = dbo.{name};"
            )
        ));
        sb.AppendLine("}");
        
        return sb.ToString();
    }

    protected override void Generate(PamelloEntityDescriptor descriptor, StringBuilder generatorSb) {
        generatorSb.AppendLine(
            SharedHelper.WriteInsideType(
                descriptor.InvokingType,
                "",
                $$"""
                {{(descriptor.NeedsIdGenerated
                    ? descriptor.IsDatabaseEntity
                        ? """
                        private int _id = 0;
                        public override int Id => _id;
                        """
                        : """
                        private static int _idCounter = 0;

                        private int _id = ++_idCounter;
                        public override int Id => _id;
                        """
                    : ""
                )}}
                
                {{GetEntityDboConstructor(descriptor)}}
                
                {{GetUpdatablePropertiesSource(descriptor)}}
                {{GetEntityDtoAndDboClassesSource(descriptor)}}
                {{GetEntityDtoAndDboGetter(descriptor)}}
                """
            ).ToString()
        );
    }
}
