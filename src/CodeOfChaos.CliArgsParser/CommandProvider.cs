// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

namespace CodeOfChaos.CliArgsParser;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CommandProvider : ICommandProvider {
    private readonly Dictionary<string, Type> _commands = new();
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public bool TryRegisterCommand(string name, Type type) {
        return _commands.TryAdd(name, type);
    }

    public bool TryGetCommand(string name, [NotNullWhen(true)] out Type? commandType) {
        return _commands.TryGetValue(name, out commandType);
    }
}
