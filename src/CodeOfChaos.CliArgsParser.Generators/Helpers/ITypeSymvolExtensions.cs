// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.CodeAnalysis;
using System.Linq;

namespace CodeOfChaos.CliArgsParser.Generators.Helpers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class ITypeSymvolExtensions {
    public static bool HasInterfaceWithDisplayName<TSymbol>(this TSymbol symbol, string displayName) where TSymbol : ITypeSymbol {
        return symbol.AllInterfaces.Any(i => i.IsDisplayName(displayName));
    }
}
