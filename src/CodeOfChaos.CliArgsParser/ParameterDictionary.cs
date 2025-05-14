// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.RegularExpressions;

namespace CodeOfChaos.CliArgsParser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class ParameterDictionary : IParameterDictionary {
    private readonly Dictionary<string, object> _parameters = new();
    private uint _positionalCounter;
    private uint _quotedStringCounter;

    [GeneratedRegex("""(?:(?<keyValue>(?<key>--[\w\-_]+|-[\w\-_])\s*=\s*(?<value>"[^"]*"|[^ ]+)))|(?<flag>(?:--[\w\-_]+|-[\w\-_])(?=\s|$))|(?<quotedString>"(?<quoted>[^"]*)")|(?<positional>\S+)""")]
    private static partial Regex GatherValuesRegex { get; }

    public void Dispose() {
        _parameters.Clear();
        _positionalCounter = 0;
        _quotedStringCounter = 0;
        GC.SuppressFinalize(this);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    private ParameterDictionary() { }
    
    public static ParameterDictionary FromString(string input) {
        MatchCollection matches = GatherValuesRegex.Matches(input);
        var parameters = new ParameterDictionary();

        foreach (Match match in matches) {
            if (match.Groups["keyValue"].Success) {
                // Key-Value Pair
                string key = match.Groups["key"].Value;
                string value = match.Groups["value"].Value;

                // Check if the value is a boolean true/false
                if (bool.TryParse(value, out bool boolResult)) {
                    // Store as boolean
                    parameters._parameters[key] = boolResult;
                    continue;
                }

                // Remove quotations if present
                // Store as string or other data type
                parameters._parameters[key] = value.Trim('"');
                continue;
            }

            // Single Flag
            if (match.Groups["flag"].Success) {
                // Flags are interpreted as true by default
                string flag = match.Groups["flag"].Value;
                parameters._parameters[flag] = true;
                continue;
            }

            // Quoted String
            if (match.Groups["quotedString"].Success) {
                string quotedContent = match.Groups["quoted"].Value;
                parameters._parameters[$"quotedString_{parameters._quotedStringCounter++}"] = quotedContent;
                continue;
            }

            // ReSharper disable once InvertIf
            // Positional Argument
            if (match.Groups["positional"].Success) {
                string positional = match.Groups["positional"].Value;
                parameters._parameters[$"positional_{parameters._positionalCounter++}"] = positional;
            }
        }
        return parameters;
    }

    public T GetParameterByPossibleNames<T>(string name, string shortName) {
        if (_parameters.TryGetValue(name, out object? parameter) || _parameters.TryGetValue(shortName, out parameter)
        ) return (T)Convert.ChangeType(parameter, typeof(T));

        throw new KeyNotFoundException($"Parameter '{name}' or '{shortName}' not found.");
    }

    public T? GetOptionalParameterByPossibleNames<T>(string name, string shortName) {
        if (!_parameters.TryGetValue(name, out object? parameter) && !_parameters.TryGetValue(shortName, out parameter)) return default;
        if (parameter is T tValue) return tValue;

        // Handle nullable value types
        Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        try {
            return (T)Convert.ChangeType(parameter, targetType);
        }
        catch {
            return default;// Return default if conversion fails
        }
    }

    public T GetParameter<T>(string key) {
        if (!_parameters.TryGetValue(key, out object? parameter)) throw new KeyNotFoundException($"Parameter '{key}' not found.");

        return (T)Convert.ChangeType(parameter, typeof(T));
    }

    public T? GetOptionalParameter<T>(string key) {
        if (!_parameters.TryGetValue(key, out object? parameter)) return default;

        return (T)Convert.ChangeType(parameter, typeof(T));
    }
}
