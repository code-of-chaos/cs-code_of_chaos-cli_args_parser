// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

namespace CodeOfChaos.CliArgsParser.Generators;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class TypeNames {
    public const string ICliCommandInterface = "CodeOfChaos.CliArgsParser.ICliCommand";
    public const string ICliCommandGenericInterface = "CodeOfChaos.CliArgsParser.ICliCommand`1";
    public const string ICliParametersInterface = "CodeOfChaos.CliArgsParser.ICliParameters";
}
