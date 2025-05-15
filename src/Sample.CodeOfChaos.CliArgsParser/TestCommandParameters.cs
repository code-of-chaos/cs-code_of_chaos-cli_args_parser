// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser;

namespace Sample.CodeOfChaos.CliArgsParser;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public record TestCommandParameters : ICliParameters {
    [CliData, AutoName] public string TestString { get; init; } = string.Empty;
    [CliData, AutoName] public int TestInt { get; init; }
    [CliData, AutoName] public bool TestBool { get; init; }
    [CliData, AutoName] public required string TestRequiredString { get; init; }
}
