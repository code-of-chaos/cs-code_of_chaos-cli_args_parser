// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace CodeOfChaos.CliArgsParser.Generators.Sample;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public record TestCommandParameters : ICliParameters{
    [CliData, AutoName] public string TestString { get; init; }
    [CliData, AutoName] public int TestInt { get; init; }
    [CliData, AutoName] public bool TestBool { get; init; }
    [CliData, AutoName] public required string TestRequiredString { get; init; }
}
