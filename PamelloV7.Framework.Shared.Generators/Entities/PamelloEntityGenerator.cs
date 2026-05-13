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
    public static string PamelloBasicEntityAttributeName = "PamelloBasicEntityAttribute";
    public static string PamelloDtoClassFullName = "global::PamelloV7.Framework.Shared.Entities.Dto.PamelloBasicEntityDto";
    
    protected override bool Predicate(SyntaxNode node, CancellationToken cancellationToken)
        => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };

    protected override PamelloEntityDescriptor? GetDescriptorInternal(
        GeneratorSyntaxContext context,
        INamedTypeSymbol targetType,
        StringBuilder debugOutput
    ) {
        var attribute = targetType.GetAttributes()
            .FirstOrDefault(a => SharedHelper.CheckTypeName(a.AttributeClass, PamelloBasicEntityAttributeName));
        
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

    public static string GetEntityDtoSource(PamelloEntityDescriptor descriptor) {
        var dtoClassSb = new StringBuilder();
        var dtoGetterSb = new StringBuilder();
        
        dtoClassSb.AppendLine($"public partial class Dto : {PamelloDtoClassFullName} {{");

        foreach (var propertyDescriptor in descriptor.UpdatableProperties) {
            dtoClassSb.AppendLine(
                $"{SharedHelper.Tab(1)}public required {
                    propertyDescriptor.Property.Type.GetFullName()
                } {
                    propertyDescriptor.Property.Name
                } {{ get; set; }}"
            );
        }
        
        dtoClassSb.AppendLine("}");

        dtoGetterSb.AppendLine($"public override Dto GetDto() => new Dto() {{");
        dtoGetterSb.AppendLine(string.Join(",\n", descriptor.UpdatableProperties.Select(p => 
            $"{SharedHelper.Tab(1)}{p.Property.Name} = {p.Property.Name}"
        )));
        dtoGetterSb.AppendLine("};");

        return $"{dtoClassSb}\n\n{dtoGetterSb}";
    }

    protected override void Generate(PamelloEntityDescriptor descriptor, StringBuilder generatorSb) {
        generatorSb.AppendLine(
            SharedHelper.WriteInsideType(
                descriptor.InvokingType,
                "",
                $$"""
                {{(descriptor.NeedsIdGenerated ? """
                private static int _idCounter = 0;
                
                public override int Id { get; } = ++_idCounter;
                
                """ : "")}}
                
                {{GetUpdatablePropertiesSource(descriptor)}}
                
                {{GetEntityDtoSource(descriptor)}}
                """
            ).ToString()
        );
    }
}
