// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser;

namespace Tests.CodeOfChaos.CliArgsParser;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CliParserTests {
    [Test]
    public async Task ExecuteAsync_Test() {
        // Arrange
        const string input = "test-command --test-required-string=\"something\"";
        ICliParser parser = CliParser.FromBuilder()
            .AddCommandsFromAssembly(typeof(CliParserTests).Assembly)
            .Build();
        
        // Act
        await parser.ExecuteAsync(input);
        
        // Assert
        
    }
}
