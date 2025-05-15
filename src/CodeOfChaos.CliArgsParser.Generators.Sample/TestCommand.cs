// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeOfChaos.CliArgsParser.Generators.Sample;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[CliData, AutoName]
public partial class TestCommand : ICliCommand<TestCommandParameters> {
    public async Task ExecuteAsync(TestCommandParameters parameters, CancellationToken ct = default) {
        throw new NotImplementedException();
    }
}
