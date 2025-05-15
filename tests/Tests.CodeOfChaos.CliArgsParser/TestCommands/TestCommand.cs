// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser;

namespace Tests.CodeOfChaos.CliArgsParser.TestCommands;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[CliData, AutoName]
public partial class TestCommand : ICliCommand<TestCommandParameters> {
    public ValueTask ExecuteAsync(TestCommandParameters parameters, CancellationToken ct = default) {
        return ValueTask.CompletedTask;
    }
}
