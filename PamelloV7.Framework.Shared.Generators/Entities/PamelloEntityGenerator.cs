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
    public const string PamelloDaoClassFullName = "global::PamelloV7.Framework.Core.Entities.Dao.PamelloBasicDao";
    
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
                ? $"{SharedHelper.GetSymbolModifiers(propertyDescriptor.Property.GetMethod)} "
                : ""
            )} get => {ToPrivateFieldName(propertyDescriptor.Property.Name)};
            """;
    }
    
    public static string WriteUpdatablePropertySetter(PamelloEntityDescriptor descriptor, PamelloEntityUpdatablePropertyDescriptor propertyDescriptor) {
        if (propertyDescriptor.Property.SetMethod is null) return "//no setter";
        
        return
            $$"""
            {{(propertyDescriptor.Property.SetMethod.DeclaredAccessibility < propertyDescriptor.Property.DeclaredAccessibility
                ? $"{SharedHelper.GetSymbolModifiers(propertyDescriptor.Property.SetMethod)} "
                : ""
            )}}

            {{(descriptor.IsDatabaseEntity
                ? $$$"""
                set {
                    if ({{{ToPrivateFieldName(propertyDescriptor.Property.Name)}}} == value) return;
                    
                    {{{ToPrivateFieldName(propertyDescriptor.Property.Name)}}} = value;
                    
                    Save();
                }
                """
                : $$$"""
                set => {{{ToPrivateFieldName(propertyDescriptor.Property.Name)}}} = value;
                """
            )}}
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
                    {{WriteUpdatablePropertySetter(descriptor, propertyDescriptor)}}
                }
                
                """
            );
        }
        
        return updatablePropertiesSb.ToString();
    }

    public static string GetEntityDtoAndDaoClassesSource(PamelloEntityDescriptor descriptor) {
        var dtoClassSb = new StringBuilder();
        var daoClassSb = new StringBuilder();
        
        dtoClassSb.AppendLine($"public partial class Dto : {PamelloDtoClassFullName} {{");
        daoClassSb.AppendLine($"public partial class Dao : {PamelloDaoClassFullName} {{");

        foreach (var propertyDescriptor in descriptor.UpdatableProperties) {
            var line = $"{SharedHelper.Tab(1)}public required {
                propertyDescriptor.Property.Type.GetFullName()
            } {
                propertyDescriptor.Property.Name
            } {{ get; set; }}";
            
            dtoClassSb.AppendLine(line);
            daoClassSb.AppendLine(line);
        }
        
        dtoClassSb.AppendLine("}");
        daoClassSb.AppendLine("}");
        
        if (!descriptor.IsDatabaseEntity) {
            daoClassSb.Clear();
            daoClassSb.AppendLine($"//no dao class required");
        }
        
        return $"{dtoClassSb}\n{daoClassSb}";
    }

    public static string GetEntityDtoAndDaoGetter(PamelloEntityDescriptor descriptor) {
        var dtoGetterSb = new StringBuilder();
        var daoGetterSb = new StringBuilder();

        dtoGetterSb.AppendLine($"public override Dto GetDto() => new Dto() {{");
        daoGetterSb.AppendLine($"public override Dao GetDao() => new Dao() {{");
        
        dtoGetterSb.AppendLine(string.Join(",\n", descriptor.UpdatableProperties
            .Select(p => p.Property.Name)
            .Prepend("Id")
            .Select(name => 
                $"{SharedHelper.Tab(1)}{name} = {name}"
            )
        ));
        daoGetterSb.AppendLine(string.Join(",\n", descriptor.UpdatableProperties
            .Select(p => p.Property.Name)
            .Prepend("Id")
            .Select(name => 
                $"{SharedHelper.Tab(1)}{name} = {ToPrivateFieldName(name)}"
            )
        ));
        
        dtoGetterSb.AppendLine("};");
        daoGetterSb.AppendLine("};");

        if (!descriptor.IsDatabaseEntity) {
            daoGetterSb.Clear();
            daoGetterSb.AppendLine($"//no dao getter required");
        }
        
        return $"{dtoGetterSb}\n{daoGetterSb}";
    }

    public static string GetEntityDaoConstructor(PamelloEntityDescriptor descriptor) {
        if (!descriptor.IsDatabaseEntity) return "//no dao constructor required";
        var sb = new StringBuilder();
        
        sb.AppendLine($"public {descriptor.InvokingType.Name}(Dao dao) : base() {{");
        sb.AppendLine(string.Join("\n", descriptor.UpdatableProperties
            .Select(p => p.Property.Name)
            .Prepend("Id")
            .Select(name => 
                $"{SharedHelper.Tab(1)}{ToPrivateFieldName(name)} = dao.{name};"
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
                        //no need to generate id for database entity
                        """
                        : """
                        private static int _idCounter = 0;

                        private int _id = ++_idCounter;
                        public override int Id => _id;
                        """
                    : ""
                )}}
                
                {{GetEntityDaoConstructor(descriptor)}}
                
                {{GetUpdatablePropertiesSource(descriptor)}}
                {{GetEntityDtoAndDaoClassesSource(descriptor)}}
                {{GetEntityDtoAndDaoGetter(descriptor)}}
                """
            ).ToString()
        );
    }
}
