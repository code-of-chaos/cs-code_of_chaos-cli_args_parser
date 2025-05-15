// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace CodeOfChaos.CliArgsParser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class CliParser : ICliParser {
    public required Lazy<IServiceProvider>? ServiceProvider { get; init; }
    public required ICommandProvider CommandProvider { get; init; }

    private ILogger<CliParser>? Logger => ServiceProvider?.Value.GetService<ILogger<CliParser>>();

    [GeneratedRegex(@"\s+")] private static partial Regex FindEmptySpacesRegex { get; }
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    internal CliParser() {}
    public static CliParserBuilder CreateBuilder() {
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

        if (!CommandProvider.TryGetCommand(commandName, out Type? commandType)) {
            if (Logger is null) throw new Exception($"Could not find command by name {commandName}");
            Logger.LogWarning("Could not find command by name {name}", commandName);
            return;
        }

        if (!TryCreateCommandInstance(commandType, out ICliCommand? command)) {
            if (Logger is null) throw new Exception($"Could not instantiate command with type {commandType.FullName}");
            Logger.LogWarning("Could not instantiate command with type {type}", commandType.FullName);
            return;
            
        }

        ParameterDictionary parameterDictionary = ParameterDictionary.FromString(parameterInput);
        await command.StartExecution(parameterDictionary, ct);
    }

    private bool TryCreateCommandInstance(Type commandType, [NotNullWhen(true)] out ICliCommand? command) {
        try {
            if (ServiceProvider is null) {
                object? directInstance = Activator.CreateInstance(commandType);
                command = directInstance as ICliCommand;
                return command is not null;
            }

            IServiceScope scope = ServiceProvider.Value.CreateScope();
            object diInstance = ActivatorUtilities.CreateInstance(scope.ServiceProvider, commandType);
            command = diInstance as ICliCommand;
            return command is not null;
        }
        catch (Exception e) {
            // Throw if we cant log
            if (Logger is null) throw;
            Logger.LogWarning(e, "Failed to create command instance");
            command = null;
            return false;
        }
    }
}
