// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.RegularExpressions;

namespace CodeOfChaos.CliArgsParser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class CliParser : ICliParser {
    public required Lazy<IServiceProvider>? ServiceProvider { get; init; }
    public required ICommandProvider CommandProvider { get; init; }
    
    [GeneratedRegex(@"\s+")] private static partial Regex FindEmptySpacesRegex { get; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    internal CliParser() { }
    public static CliParserBuilder FromBuilder() {
        return new CliParserBuilder();
    }
    
    public ValueTask ExecuteAsync(string[] args, CancellationToken ct = default) => ExecuteAsync(ArgsInputHelper.ToOneLine(args), ct);
    public async ValueTask ExecuteAsync(string input, CancellationToken ct = default) {
        input = input.Replace("\\\"", "\"");
        if (input.Contains("&&")) {
            IEnumerable<Task> tasks = input
                .Split("&&")
                .Select(section => ExecuteAsync(section).AsTask());
            await Task.WhenAll(tasks);
            return;
        }
        
        string[] tokens = FindEmptySpacesRegex.Split(input);
        string commandName = tokens[0];
        string parameterInput = string.Join(" ", tokens.Skip(1));

        if (!CommandProvider.TryGetCommand(commandName, out Type? commandType)) throw new Exception("no Command found");

        ICliCommand? command = ServiceProvider is not null
            ? Activator.CreateInstance(commandType, ServiceProvider.Value) as ICliCommand
            : Activator.CreateInstance(commandType) as ICliCommand;
        
        if (command is null) throw new Exception("no Command found");
        
        ParameterDictionary parameterDictionary = ParameterDictionary.FromString(parameterInput);
        await command.StartExecution(parameterDictionary, ct);
    }
}
