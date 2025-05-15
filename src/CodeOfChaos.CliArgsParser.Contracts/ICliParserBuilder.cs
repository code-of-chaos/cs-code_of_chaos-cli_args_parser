// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Reflection;

namespace CodeOfChaos.CliArgsParser;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface ICliParserBuilder {
    ICliParserBuilder WithServiceProvider(IServiceProvider provider);
    ICliParserBuilder WithServiceProvider(Func<IServiceProvider> provider);
    ICliParserBuilder AddFromAssembly<TEntrypoint>();
    ICliParserBuilder AddFromAssembly(Assembly assembly);
    
    ICliParser Build();
}
