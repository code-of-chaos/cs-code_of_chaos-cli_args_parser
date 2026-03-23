// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser;
using JetBrains.Annotations;

namespace Tests.CodeOfChaos.CliArgsParser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[TestSubject(typeof(ParameterDictionary))]
public class ParameterDictionaryTests {
    [Test]
    public async Task Test_IngestString_ParsesSingleKeyValuePair() {
        // Arrange
        const string input = "--key=value";

        // Act
        ParameterDictionary registry = ParameterDictionary.FromString(input);

        // Assert
        await Assert.That(registry.GetParameter<string>("--key")).IsNotNull().Because("The parameter should exist");
    }

    [Test]
    public async Task Test_IngestString_ParsesMultipleKeyValuePairs() {
        // Arrange
        const string input = """--key1=value1 --key2="value 2" """;

        // Act
        ParameterDictionary registry = ParameterDictionary.FromString(input);
        string keyValue1 = registry.GetParameter<string>("--key1");
        string keyValue2 = registry.GetParameter<string>("--key2");

        // Assert
        await Assert.That(keyValue1).IsNotNull().Because("The parameter should exist");
        await Assert.That(keyValue2).IsNotNull().Because("The parameter should exist");
        await Assert.That(keyValue1).IsEqualTo("value1");
        await Assert.That(keyValue2).IsEqualTo("value 2");
    }

    [Test]
    [Arguments("--flag")]
    // [Arguments( "-f" )]
    public async Task Test_IngestString_ParsesFlags(string input) {
        // Arrange

        // Act
        ParameterDictionary registry = ParameterDictionary.FromString(input);
        bool flag = registry.GetParameter<bool>("--flag");

        // Assert
        await Assert.That(flag as object).IsNotNull().Because("The parameter should exist");
        await Assert.That(flag).IsTrue();
    }

    [Test]
    public async Task Test_IngestString_ParsesQuotedString() {
        // Arrange
        string input = "\"This is a test string\"";

        // Act
        ParameterDictionary registry = ParameterDictionary.FromString(input);

        // Assert
        await Assert.That(registry.GetParameter<string>("quotedString_0")).IsNotNull().Because("The parameter should exist");
    }

    [Test]
    public async Task Test_IngestString_ParsesPositionalArguments() {
        // Arrange
        const string input = "arg1 arg2";

        // Act
        ParameterDictionary registry = ParameterDictionary.FromString(input);
        string positional0 = registry.GetParameter<string>("positional_0");
        string positional1 = registry.GetParameter<string>("positional_1");

        // Assert
        await Assert.That(positional0).IsNotNull();
        await Assert.That(positional1).IsNotNull();
        await Assert.That(positional0).IsEqualTo("arg1");
        await Assert.That(positional1).IsEqualTo("arg2");
    }

    [Test]
    public async Task Test_GetParameter_ThrowsKeyNotFoundException_WhenParameterNotFound() {
        // Arrange
        ParameterDictionary registry = ParameterDictionary.FromString(string.Empty);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => Task.FromResult(registry.GetParameter<string>("nonexistent")));
    }

    [Test]
    public async Task Test_GetOptionalParameter_ReturnsNull_WhenParameterNotFound() {
        // Arrange
        ParameterDictionary registry = ParameterDictionary.FromString(string.Empty);

        // Act
        string? result = registry.GetOptionalParameter<string>("nonexistent");

        // Assert
        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task Test_GetOptionalParameter_ReturnsValue_WhenParameterExists() {
        // Arrange
        ParameterDictionary registry = ParameterDictionary.FromString("--key=value");

        // Act
        string? result = registry.GetOptionalParameter<string>("--key");

        // Assert
        await Assert.That(result).IsEqualTo("value");
    }

    [Test]
    public async Task Test_IngestString_ParsesComplexInputWithMixedArguments() {
        // Arrange
        const string input = "--key1=value1 --key2=value2 -f --quoted=\"This is a test\" arg1 arg2 --boolFlag=true --negativeFlag=false";
        ParameterDictionary registry = ParameterDictionary.FromString(input);

        // Act
        string key1 = registry.GetParameter<string>("--key1");
        string key2 = registry.GetParameter<string>("--key2");
        bool shortFlag = registry.GetParameter<bool>("-f");
        string quotedString = registry.GetParameter<string>("--quoted");
        string positional0 = registry.GetParameter<string>("positional_0");
        string positional1 = registry.GetParameter<string>("positional_1");
        bool boolFlag = registry.GetParameter<bool>("--boolFlag");
        bool negativeFlag = registry.GetParameter<bool>("--negativeFlag");

        // Assert
        await Assert.That(key1).IsNotNull().Because("The parameter 'key1' should exist");
        await Assert.That(key1).IsEqualTo("value1");

        await Assert.That(key2).IsNotNull().Because("The parameter 'key2' should exist");
        await Assert.That(key2).IsEqualTo("value2");

        await Assert.That(shortFlag as object).IsNotNull().Because("The flag '-f' should be a valid boolean value");
        await Assert.That(shortFlag).IsTrue();

        await Assert.That(quotedString).IsNotNull().Because("The quoted string should be parsed correctly");
        await Assert.That(quotedString).IsEqualTo("This is a test");

        await Assert.That(positional0).IsNotNull().Because("The first positional argument should exist");
        await Assert.That(positional0).IsEqualTo("arg1");

        await Assert.That(positional1).IsNotNull().Because("The second positional argument should exist");
        await Assert.That(positional1).IsEqualTo("arg2");

        await Assert.That(boolFlag as object).IsNotNull().Because("The boolean flag 'boolFlag' should exist");
        await Assert.That(boolFlag).IsTrue();

        await Assert.That(negativeFlag as object).IsNotNull().Because("The boolean flag 'negativeFlag' should exist");
        await Assert.That(negativeFlag).IsFalse();
    }

    [Test]
    public async Task Test_GetParameterByPossibleNames_ReturnsValue_WhenFullNameExists() {
        // Arrange
        ParameterDictionary registry = ParameterDictionary.FromString("--fullName=value");

        // Act
        string result = registry.GetParameterByPossibleNames<string>("--fullName", "-f");

        // Assert
        await Assert.That(result).IsEqualTo("value");
    }

    [Test]
    public async Task Test_GetParameterByPossibleNames_ReturnsValue_WhenShortNameExists() {
        // Arrange
        ParameterDictionary registry = ParameterDictionary.FromString("-f=value");

        // Act
        string result = registry.GetParameterByPossibleNames<string>("--fullName", "-f");

        // Assert
        await Assert.That(result).IsEqualTo("value");
    }

    [Test]
    public async Task Test_GetParameterByPossibleNames_ThrowsKeyNotFoundException_WhenParameterNotFound() {
        // Arrange
        var registry = ParameterDictionary.FromString(string.Empty);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            Task.FromResult(registry.GetParameterByPossibleNames<string>("--nonexistent", "-n")));
    }

    [Test]
    public async Task Test_GetOptionalParameterByPossibleNames_ReturnsValue_WhenFullNameExists() {
        // Arrange
        ParameterDictionary registry = ParameterDictionary.FromString("--fullName=value");

        // Act
        string? result = registry.GetOptionalParameterByPossibleNames<string>("--fullName", "-f");

        // Assert
        await Assert.That(result).IsEqualTo("value");
    }

    [Test]
    public async Task Test_GetOptionalParameterByPossibleNames_ReturnsValue_WhenShortNameExists() {
        // Arrange
        ParameterDictionary registry = ParameterDictionary.FromString("-f=value");

        // Act
        string? result = registry.GetOptionalParameterByPossibleNames<string>("--fullName", "-f");

        // Assert
        await Assert.That(result).IsEqualTo("value");
    }

    [Test]
    public async Task Test_GetOptionalParameterByPossibleNames_ReturnsNull_WhenParameterNotFound() {
        // Arrange
        ParameterDictionary registry = ParameterDictionary.FromString(string.Empty);

        // Act
        string? result = registry.GetOptionalParameterByPossibleNames<string>("--nonexistent", "-n");

        // Assert
        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task Test_GetParameterByPossibleNames_BooleanFlag_ReturnsValue() {
        // Arrange
        ParameterDictionary registry = ParameterDictionary.FromString("--flag");

        // Act
        bool flag = registry.GetParameterByPossibleNames<bool>("--flag", "-f");

        // Assert
        await Assert.That(flag).IsTrue();
    }

    [Test]
    public async Task Test_GetOptionalParameterByPossibleNames_BooleanFlag_ReturnsValue() {
        // Arrange
        ParameterDictionary registry = ParameterDictionary.FromString("--flag");

        // Act
        bool? flag = registry.GetOptionalParameterByPossibleNames<bool>("--flag", "-f");

        // Assert
        await Assert.That(flag).IsTrue();
    }

    [Test]
    public async Task Test_GetKeyValue_WithHyphen_ReturnsValue() {
        // Arrange
        ParameterDictionary registry = ParameterDictionary.FromString("--key-value=\"value\"");

        // Act
        string? result = registry.GetOptionalParameter<string>("--key-value");

        // Assert
        await Assert.That(result).IsEqualTo("value");
    }
}
