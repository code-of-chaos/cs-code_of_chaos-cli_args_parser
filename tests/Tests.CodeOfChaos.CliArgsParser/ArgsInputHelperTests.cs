// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser;

namespace Tests.CodeOfChaos.CliArgsParser;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ArgsInputHelperTests {
    [Test]
    public async Task ToOneLine_EmptyInput_ReturnsEmptyString() {
        // Arrange
        string[] input = Array.Empty<string>();
        
        // Act
        string result = ArgsInputHelper.ToOneLine(input);
        
        // Assert
        await Assert.That(result).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task ToOneLine_SingleArgument_ReturnsUnchanged() {
        // Arrange
        string[] input = ["simple"];
        
        // Act
        string result = ArgsInputHelper.ToOneLine(input);
        
        // Assert
        await Assert.That(result).IsEqualTo("simple");
    }

    [Test]
    public async Task ToOneLine_MultipleSimpleArguments_JoinsWithSpaces() {
        // Arrange
        string[] input = ["arg1", "arg2", "arg3"];
        
        // Act
        string result = ArgsInputHelper.ToOneLine(input);
        
        // Assert
        await Assert.That(result).IsEqualTo("arg1 arg2 arg3");
    }

    [Test]
    public async Task ToOneLine_ArgumentWithSpaces_AddsQuotes() {
        // Arrange
        string[] input = ["argument with spaces"];
        
        // Act
        string result = ArgsInputHelper.ToOneLine(input);
        
        // Assert
        await Assert.That(result).IsEqualTo("\"argument with spaces\"");
    }

    [Test]
    public async Task ToOneLine_KeyValuePair_AddsQuotesToValue() {
        // Arrange
        string[] input = ["key=value"];
        
        // Act
        string result = ArgsInputHelper.ToOneLine(input);
        
        // Assert
        await Assert.That(result).IsEqualTo("key=\"value\"");
    }

    [Test]
    public async Task ToOneLine_AlreadyQuotedValue_PreservesQuotes() {
        // Arrange
        string[] input = ["key=\"value\""];
        
        // Act
        string result = ArgsInputHelper.ToOneLine(input);
        
        // Assert
        await Assert.That(result).IsEqualTo("key=\"value\"");
    }

    [Test]
    public async Task ToOneLine_FlagArgument_RemainsUnchanged() {
        // Arrange
        string[] input = ["-flag", "--longflag"];
        
        // Act
        string result = ArgsInputHelper.ToOneLine(input);
        
        // Assert
        await Assert.That(result).IsEqualTo("-flag --longflag");
    }

    [Test]
    public async Task ToOneLine_ComplexMixedArguments_HandlesCorrectly() {
        // Arrange
        string[] input = [
            "command",
            "-f",
            "--long-flag",
            "simple",
            "key=value with spaces",
            "text with spaces",
            "quoted=\"already quoted\""
        ];
        
        // Act
        string result = ArgsInputHelper.ToOneLine(input);
        
        // Assert
        await Assert.That(result).IsEqualTo("command -f --long-flag simple key=\"value with spaces\" \"text with spaces\" quoted=\"already quoted\"");
    }
}