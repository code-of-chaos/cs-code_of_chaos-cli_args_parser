// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser;
using System.Threading.Tasks;

namespace Sample.CodeOfChaos.CliArgsParser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class Program {
    public static async Task Main(string[] args) {
        ICliParser parser = CliParser.FromBuilder()
            .AddCommandsFromAssembly<TestCommand>()
            .Build();

        await parser.ExecuteAsync(args);
    }
}
