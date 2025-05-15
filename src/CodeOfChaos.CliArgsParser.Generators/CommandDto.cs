// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser.Generators.Helpers;
using CodeOfChaos.GeneratorTools;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace CodeOfChaos.CliArgsParser.Generators;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public record CommandDto(
    [UsedImplicitly] INamedTypeSymbol Symbol,
    ITypeSymbol ParameterType
) {
    public string ClassName { get; } = Symbol.Name;
    public string NameSpace { get; } = Symbol.ContainingNamespace.ToDisplayString();
    public string Accessibility { get; } = Symbol.GetAccessibility();
    public string TypeKeyword { get; } = Symbol.GetTypeKind();
    
    public ParameterDto[] Parameters => GetParameters(ParameterType);
    public string ParameterTypeDisplayString { get; } = ParameterType.ToDisplayString();
    
    public string Name { get; } = GetName(Symbol).ToQuotedString();
    public string ShortName { get; } = GetShortName(Symbol).ToQuotedString();
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    private static string GetName(INamedTypeSymbol symbol) {
        ImmutableArray<AttributeData> attributes = symbol.GetAttributes();
        AttributeData? cliDataAttribute = attributes.FirstOrDefault(attr => attr.IsDisplayName(TypeNames.CliDataAttribute));

        if (cliDataAttribute?.ConstructorArguments.ElementAtOrDefault(0).Value is string cliDataName) return cliDataName;
        return attributes.Any(attr => attr.IsDisplayName(TypeNames.AutoNameAttribute)) 
            ? string.Join("", symbol.Name.ToKebabCase().Split('-').Select(s => s[0]))
            : "UNDEFINED";
    }

    private static string GetShortName(INamedTypeSymbol symbol) {
        ImmutableArray<AttributeData> attributes = symbol.GetAttributes();
        AttributeData? cliDataAttribute = attributes.FirstOrDefault(attr => attr.IsDisplayName(TypeNames.CliDataAttribute));

        if (cliDataAttribute?.ConstructorArguments.ElementAtOrDefault(1).Value is string cliDataName) return cliDataName;
        return attributes.Any(attr => attr.IsDisplayName(TypeNames.AutoNameAttribute)) 
            ? string.Join("", symbol.Name.ToKebabCase().Split('-').Select(s => s[0]))
            : "UNDEFINED";
    }
    
    public static CommandDto? TryCreate(INamedTypeSymbol symbol) {
        if (!symbol.HasAttributeWithDisplayName(TypeNames.CliDataAttribute)) return null;

        ImmutableArray<INamedTypeSymbol> interfaces = symbol.AllInterfaces;
        ITypeSymbol? parameterType = interfaces
            .Where(i => i.IsGenericType && i.ConstructedFrom.IsDisplayName(TypeNames.ICliCommandGenericInterface))
            .Select(i => i.TypeArguments[0])
            .FirstOrDefault();
        
        if (parameterType is null) return null;
        
        return new CommandDto(symbol, parameterType);

    }

    private ParameterDto[] GetParameters(ITypeSymbol symbol) {
        return symbol.GetMembers()
            .Where(member => member is IPropertySymbol propertySymbol
                && propertySymbol.HasAttributeWithDisplayName(TypeNames.CliDataAttribute))
            .Select(member => new ParameterDto((member as IPropertySymbol)!))
            .ToArray();
    }

    public void CreatePartialClass(GeneratorStringBuilder builder) {
        builder.AppendUsings(
            "System",
            "System.Threading",
            "System.Threading.Tasks",
            "CodeOfChaos.CliArgsParser"
        );
        builder.AppendNamespace(NameSpace);
        builder.AppendLine();
        builder.AppendNullableEnable();
        builder.AppendLine($"public partial {TypeKeyword} {ClassName} {{");
        builder.Indent(b => {
            b.AppendLine("public ValueTask StartExecution(IParameterDictionary parameterDictionary, CancellationToken ct = default){");
            b.Indent(b1 => {
                b1.AppendLine($"var parameters = new {ParameterTypeDisplayString}(){{");
                b1.ForEachAppendLineIndented(Parameters, parameterDto => $"{parameterDto.PropertyName} = {parameterDto.GetWithPropertyDictionary()},");
                b1.AppendLine("};");
                b1.AppendLine("return ExecuteAsync(parameters, ct);");
            });
            b.AppendLine("}");
        });
        builder.AppendLine("}");
    }
}
