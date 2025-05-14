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
    public async Task ExecuteAsync(TestCommandParameters parameters, CancellationToken ct = default) {
        throw new NotImplementedException();
    }
}
