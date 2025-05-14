// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace CodeOfChaos.CliArgsParser;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface ICliArgsParserBuilder {
    ICliArgsParserBuilder AddServices(IServiceProvider provider);
    ICliArgsParserBuilder AddServices(Func<IServiceProvider> provider);
    
    ICliArgsParser Build();
}
