// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Frozen;
using System.Text.RegularExpressions;

namespace CodeOfChaos.CliArgsParser.OLD;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class CliArgsParser {
    public required FrozenDictionary<string, (CommandData, INonGenericCommandInterfaces)> CommandLookup { get; init; }
    public (CommandData CommandData, INonGenericCommandInterfaces CommandObject)? StartupCommand { get; init; }

    public required bool HasCustomExitCommand { get; init; }
    public required bool HasCustomHelpCommand { get; init; }

    [GeneratedRegex(@"\s+")]
    private static partial Regex FindEmptySpacesRegex { get; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public Task ParseAsync(string[] args) => ParseAsync(ArgsInputHelper.ToOneLine(args));

    public async Task ParseAsync(string args) {
        if (string.IsNullOrWhiteSpace(args)) {
            throw new ArgumentException("Arguments cannot be null or empty.", nameof(args));
        }

        // Split the input into tokens
        string[] tokens = FindEmptySpacesRegex.Split(args);
        if (tokens.Length == 0 || string.IsNullOrWhiteSpace(tokens[0])) {
            throw new ArgumentException("Invalid arguments provided.", nameof(args));
        }

        // Extract the command name (assume the first token is the command name)
        string commandName = tokens[0];

        // Check if the command exists in the CommandLookup
        if (!CommandLookup.TryGetValue(commandName, out (CommandData CommandData, INonGenericCommandInterfaces CommandObject) commandEntry)) {
            throw new KeyNotFoundException($"Command '{commandName}' not found.");
        }

        // Call the InitializeAsync method
        var input = ArgsInputHelper.ToOneLine(tokens.Skip(1));
        await commandEntry.CommandObject.InitializeAsync(ParameterDictionary.FromString(input));
    }

    public Task ParseStartupArgs(string[] args) => ParseStartupArgs(string.Join(" ", args));

    public async Task ParseStartupArgs(string args) {
        if (StartupCommand is not { CommandObject: var commandObject }) throw new Exception("No startup command set.");
        
        await commandObject.InitializeAsync(ParameterDictionary.FromString(args));
    }

    public async Task StartUserInputMode() {
        // Inform user about the input mode
        Console.WriteLine("Entering interactive user input mode. Type your commands below. Type 'exit' to quit.");

        while (true) {
            // Prompt the user for input
            Console.Write("$:> ");
            string input = Console.ReadLine()?.Trim() ?? string.Empty;

            // Skip empty input
            if (string.IsNullOrWhiteSpace(input)) {
                Console.WriteLine("No command entered, please try again.");
                continue;
            }

            try {
                if (!HasCustomExitCommand && input.Equals("exit")) {
                    // TODO create a proper automagic exit command
                    Console.WriteLine("Exiting interactive user input mode.");
                    break;
                }

                if (!HasCustomHelpCommand && input.Equals("help")) {
                    // TODO create a proper automagic help command
                    Console.WriteLine("Available commands:");
                    foreach (string commandName in CommandLookup.Keys) {
                        Console.WriteLine($" - {commandName}");
                    }

                    continue;
                }

                // Parse and execute the command
                await ParseAsync(input);
            }
            catch (Exception ex) {
                // Handle possible errors during command execution
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
