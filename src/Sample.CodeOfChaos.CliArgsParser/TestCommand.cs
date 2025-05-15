// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.CodeOfChaos.CliArgsParser;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[CliData, AutoName]
public partial class TestCommand : ICliCommand<TestCommandParameters> {
    public async ValueTask ExecuteAsync(TestCommandParameters parameters, CancellationToken ct = default) {
        throw new NotImplementedException();
    }
}
