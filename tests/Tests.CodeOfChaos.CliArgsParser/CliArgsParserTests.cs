// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser;
using Microsoft.Extensions.DependencyInjection;
using Tests.CodeOfChaos.CliArgsParser.TestCommands;

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
    
    [Test]
    public async Task ExecuteAsync_TestWithServices() {
        // Arrange
        const string input = "test-service-command --test-required-string=\"something\" --test-bool";
        var services = new ServiceCollection();
        services.AddSingleton<IService, Service>();
        ServiceProvider provider = services.BuildServiceProvider();
        
        ICliParser parser = CliParser.FromBuilder()
            .AddServices(provider)
            .AddCommandsFromAssembly(typeof(CliParserTests).Assembly)
            .Build();
        
        // Act
        await parser.ExecuteAsync(input);

        // Assert
        var service = provider.GetRequiredService<IService>();
        TestCommandParameters? parameters = service.Parameters;
        await Assert.That(parameters).IsNotNull();
        await Assert.That(parameters!.TestRequiredString).IsEqualTo("something");
        await Assert.That(parameters.TestBool).IsTrue();
    }
}
