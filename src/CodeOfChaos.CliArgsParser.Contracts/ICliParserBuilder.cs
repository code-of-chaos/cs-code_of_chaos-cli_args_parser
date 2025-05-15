// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Reflection;

namespace CodeOfChaos.CliArgsParser;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface ICliParserBuilder {
    ICliParserBuilder AddServices(IServiceProvider provider);
    ICliParserBuilder AddServices(Func<IServiceProvider> provider);
    ICliParserBuilder AddCommandsFromAssembly<TEntrypoint>();
    ICliParserBuilder AddCommandsFromAssembly(Assembly assembly);
    
    ICliParser Build();
}
