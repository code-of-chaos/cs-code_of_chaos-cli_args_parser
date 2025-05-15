// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser.Generators.Helpers;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace CodeOfChaos.CliArgsParser.Generators;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public record ParameterDto([UsedImplicitly] IPropertySymbol Symbol) {
    public string PropertyName { get; } = Symbol.Name;
    public string PropertyType { get; } = Symbol.Type.ToDisplayString();
    public string NameSpace { get; } = Symbol.ContainingNamespace.ToDisplayString();
    public string Accessibility { get; } = Symbol.GetAccessibility();
    public bool IsRequired { get; } = Symbol.IsRequired;
    public bool HasInitSetter { get; } = Symbol.SetMethod?.IsInitOnly ?? false;

    public string Name { get; } = $"--{GetName(Symbol)}".ToQuotedString();
    public string ShortName { get; } = $"-{GetShortName(Symbol)}".ToQuotedString();

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    private static string GetName(IPropertySymbol symbol) {
        ImmutableArray<AttributeData> attributes = symbol.GetAttributes();
        AttributeData? cliDataAttribute = attributes.FirstOrDefault(attr => attr.IsDisplayName(TypeNames.CliDataAttribute));

        if (cliDataAttribute?.ConstructorArguments.ElementAtOrDefault(0).Value is string cliDataName) return cliDataName;
        return attributes.Any(attr => attr.IsDisplayName(TypeNames.AutoNameAttribute)) 
            ? symbol.Name.ToKebabCase()
            : "UNDEFINED";
    }

    private static string GetShortName(IPropertySymbol symbol) {
        ImmutableArray<AttributeData> attributes = symbol.GetAttributes();
        AttributeData? cliDataAttribute = attributes.FirstOrDefault(attr => attr.IsDisplayName(TypeNames.CliDataAttribute));

        if (cliDataAttribute?.ConstructorArguments.ElementAtOrDefault(1).Value is string cliDataName) return cliDataName;
        return attributes.Any(attr => attr.IsDisplayName(TypeNames.AutoNameAttribute)) 
            ? string.Join("", symbol.Name.ToKebabCase().Split('-').Select(s => s[0]))
            : "UNDEFINED";
    }


    public string GetWithPropertyDictionary() {
        if (IsRequired) return $"parameterDictionary.GetParameterByPossibleNames<{PropertyType}>({Name}, {ShortName})";

        if (PropertyType == "bool") return $"parameterDictionary.GetOptionalParameterByPossibleNames<{PropertyType}>({Name}, {ShortName})";
        
        string fallback = Symbol.DeclaringSyntaxReferences
            .Select(r => r.GetSyntax())
            .OfType<PropertyDeclarationSyntax>()
            .FirstOrDefault()?.Initializer?.Value.ToString() ?? "default";

        return $"parameterDictionary.GetOptionalParameterByPossibleNames<{PropertyType}>({Name}, {ShortName}) ?? {fallback}";
    }
}
