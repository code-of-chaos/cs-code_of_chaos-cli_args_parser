// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser;

namespace Tests.CodeOfChaos.CliArgsParser.TestCommands;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[CliData, AutoName]
public partial class TestServiceCommand(IService service) : ICliCommand<TestCommandParameters> {
    public ValueTask ExecuteAsync(TestCommandParameters parameters, CancellationToken ct = default) {
        service.Parameters = parameters;
        return ValueTask.CompletedTask;
    }
}


public class Service : IService {
    public TestCommandParameters? Parameters { get; set; }
}

public interface IService {
    TestCommandParameters? Parameters { get; set; }
}