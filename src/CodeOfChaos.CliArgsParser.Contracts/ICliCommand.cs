// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace CodeOfChaos.CliArgsParser;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface ICliCommand<in TParameter> : ICliCommand 
    where TParameter : ICliParameters 
{
    ValueTask ExecuteAsync(TParameter parameters, CancellationToken ct = default);
}

public interface ICliCommand {
    ValueTask StartExecution(IParameterDictionary parameters, CancellationToken ct = default);
}