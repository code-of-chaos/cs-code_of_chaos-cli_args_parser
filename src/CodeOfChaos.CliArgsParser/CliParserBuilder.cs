// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Reflection;
using System.Text.RegularExpressions;

namespace CodeOfChaos.CliArgsParser;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class CliParserBuilder : ICliParserBuilder {
    private Func<IServiceProvider>? ServiceProvider { get; set; }
    private CommandProvider CommandProvider { get; set; } = new();

    [GeneratedRegex("__[A-Za-z0-9]+__CliArgsParserDictionary")]
    private static partial Regex FindCommandDictionary { get; }
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public ICliParserBuilder WithServiceProvider(IServiceProvider provider) {
        ServiceProvider = () => provider;
        return this;
    }
    
    public ICliParserBuilder WithServiceProvider(Func<IServiceProvider> provider) {
        ServiceProvider = provider;
        return this;   
    }

    public ICliParserBuilder AddFromAssembly<TEntrypoint>() => AddFromAssembly(typeof(TEntrypoint).Assembly);
    
    public ICliParserBuilder AddFromAssembly(Assembly assembly) {
        Type[] types = assembly.GetTypes();
        Type? staticDictionaryType = types.FirstOrDefault(t => FindCommandDictionary.IsMatch(t.Name) && t.IsClass);
        var commandsDictionary = staticDictionaryType?.GetField("Commands")?.GetValue(null) as Dictionary<string, Type>;
        if (commandsDictionary is null) return this;

        foreach ((string key, Type value) in commandsDictionary) {
            CommandProvider.TryRegisterCommand(key, value);
        }
        
        return this;   
    }
    

    public ICliParser Build() {
        return new CliParser {
            ServiceProvider = ServiceProvider is not null ? new Lazy<IServiceProvider>(ServiceProvider) : null,
            CommandProvider = CommandProvider
        };
    }
}
