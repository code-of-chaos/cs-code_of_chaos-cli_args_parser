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
    public ValueTask ExecuteAsync(TestCommandParameters parameters, CancellationToken ct = default) {
        Console.WriteLine(parameters.TestString);
        Console.WriteLine(parameters.TestInt);
        Console.WriteLine(parameters.TestBool);
        Console.WriteLine(parameters.TestRequiredString);
        return ValueTask.CompletedTask;
    }
}
