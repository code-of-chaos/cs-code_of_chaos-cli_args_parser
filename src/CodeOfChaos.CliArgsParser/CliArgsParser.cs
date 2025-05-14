// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace CodeOfChaos.CliArgsParser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class CliArgsParser : ICliArgsParser {
    public required Lazy<IServiceProvider>? ServiceProvider { get; init; }
    public required ICommandProvider CommandProvider { get; init; }
    
    [GeneratedRegex(@"\s+")] private static partial Regex FindEmptySpacesRegex { get; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public CliArgsParserBuilder FromBuilder() {
        return new CliArgsParserBuilder();
    }
    
    public ValueTask ExecuteAsync(string[] args) => ExecuteAsync(ArgsInputHelper.ToOneLine(args));
    public async ValueTask ExecuteAsync(string input) {
        if (input.Contains("&&")) {
            IEnumerable<Task> tasks = input
                .Split("&&")
                .Select(section => ExecuteAsync(section).AsTask());
            await Task.WhenAll(tasks);
            return;
        }
        
        string[] tokens = FindEmptySpacesRegex.Split(input);
        string commandName = tokens[0];
        string parameterInput = ArgsInputHelper.ToOneLine(tokens.Skip(1));

        if (!CommandProvider.TryGetCommand(commandName, out Type? commandType)) {
            throw new Exception("no Command found");
            return;
        }

        ICliCommand? command = ServiceProvider is not null
            ? Activator.CreateInstance(commandType, ServiceProvider.Value) as ICliCommand
            : Activator.CreateInstance(commandType) as ICliCommand;
        
        if (command is null) {
            throw new Exception("no Command found");
            return;
        }
        
        
    }
}
