// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser.OLD;
using CodeOfChaos.Extensions;
using System.Reflection;

namespace CodeOfChaos.CliArgsParser;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CliArgsParserBuilder : ICliArgsParserBuilder {
    private Func<IServiceProvider>? ServiceProvider { get; set; }
    private CommandProvider CommandProvider { get; set; } = new();

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public ICliArgsParserBuilder AddServices(IServiceProvider provider) {
        ServiceProvider = () => provider;
        return this;
    }
    
    public ICliArgsParserBuilder AddServices(Func<IServiceProvider> provider) {
        ServiceProvider = provider;
        return this;   
    }

    public ICliArgsParserBuilder AddCommandsFromAssembly(Assembly assembly) {
        IEnumerable<Type> commands = assembly.GetTypes().Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>)));

        foreach (Type command in commands) {
            Attribute[] attributes = command.GetCustomAttributes().ToArray();
            var cliData = (CliDataAttribute?)attributes.FirstOrDefault(a => a is CliDataAttribute);
            string? name = cliData?.Name;
            string? shortName = cliData?.ShortName;
            
            bool hasAutoName = attributes.Any(a => a is AutoNameAttribute);
            if (hasAutoName) {
                string commandName = command.Name;
                name ??= $"--{commandName.ToKebabCase()}";
                shortName ??= $"-{string.Join("", commandName.ToKebabCase().Split('-').Select(s => s[0]))}";
            }
            
            CommandProvider.TryRegisterCommand(name!, command);
            CommandProvider.TryRegisterCommand(shortName!, command);
        }
        
        return this;   
    }
    

    public ICliArgsParser Build() {
        return new CliArgsParser {
            ServiceProvider = ServiceProvider is not null ? new Lazy<IServiceProvider>(ServiceProvider) : null,
            CommandProvider = CommandProvider
        };
    }
}
