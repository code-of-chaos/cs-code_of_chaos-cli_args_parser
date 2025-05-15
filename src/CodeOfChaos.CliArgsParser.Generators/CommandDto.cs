// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
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
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
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
}
