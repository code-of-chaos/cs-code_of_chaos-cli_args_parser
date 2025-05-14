// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.CodeAnalysis;
using System.Linq;

namespace CodeOfChaos.CliArgsParser.Generators;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public record CommandDto(INamedTypeSymbol Symbol, ITypeSymbol ParameterSymbol) {
    public string ClassName { get; } = Symbol.Name;
    public string NameSpace { get; } = Symbol.ContainingNamespace.ToDisplayString();
    public string Accessibility { get; } = Symbol.GetAccessibility();
    public string TypeKeyword { get; } = Symbol.GetTypeKind();


    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static CommandDto? TryCreate(SemanticModel semanticModel, INamedTypeSymbol symbol) {
        ITypeSymbol? parameterType = symbol.AllInterfaces
            .Where(i => i.IsGenericType && i.ConstructedFrom.ToDisplayString() == TypeNames.ICliCommandGenericInterface)
            .Select(i => i.TypeArguments[0])
            .FirstOrDefault();

        if (parameterType is null) return null;


        return new CommandDto(symbol, parameterType);
    }

    private static void GetParameters(ITypeSymbol symbol) {
        // Assuming you have a typeSymbol (INamedTypeSymbol) for TestCommandParameters
        foreach (var member in typeSymbol.GetMembers()) {
            if (member is not IPropertySymbol propertySymbol) continue;

            // Get the attributes
            AttributeData? cliDataAttribute = propertySymbol.GetAttributes()
                .FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == "CodeOfChaos.CliArgsParser.CliDataAttribute");

            AttributeData? autoNameAttribute = propertySymbol.GetAttributes()
                .FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == "CodeOfChaos.CliArgsParser.AutoNameAttribute");

            if (cliDataAttribute != null) {
                var propertyInfo = new {
                    propertySymbol.Name,
                    Type = propertySymbol.Type.ToDisplayString(),
                    propertySymbol.IsRequired,
                    HasInitSetter = propertySymbol.SetMethod?.IsInitOnly ?? false,
                    // Get CliData attribute constructor arguments if any
                    CliDataName = cliDataAttribute.ConstructorArguments.Length > 0
                        ? cliDataAttribute.ConstructorArguments[0].Value?.ToString()
                        : null,
                    CliDataShortName = cliDataAttribute.ConstructorArguments.Length > 1
                        ? cliDataAttribute.ConstructorArguments[1].Value?.ToString()
                        : null
                };
                // Use propertyInfo as needed
            }
        }
    }

}
