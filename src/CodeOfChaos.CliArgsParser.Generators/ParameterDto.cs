// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser.Generators.Helpers;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Linq;

namespace CodeOfChaos.CliArgsParser.Generators;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public record ParameterDto([UsedImplicitly] IPropertySymbol Symbol) {
    public string PropertyName { get; } = Symbol.Name;
    public string NameSpace { get; } = Symbol.ContainingNamespace.ToDisplayString();
    public string Accessibility { get; } = Symbol.GetAccessibility();
    public bool HasInitSetter { get; } = Symbol.SetMethod?.IsInitOnly ?? false;

    public string Name { get; } = GetName(Symbol);
    public string ShortName { get; } = GetShortName(Symbol);

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static string GetName(IPropertySymbol symbol) {
        ImmutableArray<AttributeData> attributes = symbol.GetAttributes();
        AttributeData? cliDataAttribute = attributes.FirstOrDefault(attr => attr.IsDisplayName(TypeNames.CliDataAttribute));

        if (cliDataAttribute?.ConstructorArguments.ElementAtOrDefault(0).Value is string cliDataName) return cliDataName;
        return attributes.Any(attr => attr.IsDisplayName(TypeNames.AutoNameAttribute)) 
            ? symbol.Name.ToKebabCase()
            : "UNDEFINED";
    }
    
    public static string GetShortName(IPropertySymbol symbol) {
        ImmutableArray<AttributeData> attributes = symbol.GetAttributes();
        AttributeData? cliDataAttribute = attributes.FirstOrDefault(attr => attr.IsDisplayName(TypeNames.CliDataAttribute));

        if (cliDataAttribute?.ConstructorArguments.ElementAtOrDefault(1).Value is string cliDataName) return cliDataName;
        return attributes.Any(attr => attr.IsDisplayName(TypeNames.AutoNameAttribute)) 
            ? string.Join("", symbol.Name.ToKebabCase().Split('-').Select(s => s[0]))
            : "UNDEFINED";
    }
    
    
}
