// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Castle.Core.Internal;
using CodeOfChaos.CliArgsParser;
using System.Reflection;

namespace Tests.CodeOfChaos.CliArgsParser.Attributes;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CliDataAttributeTests {
    [CliData] public string WithoutData { get; set; } = null!;
    [CliData("name")] public string ShouldSetName { get; set; } = null!;
    [CliData(null, "short")] public string ShouldSetShortName { get; set; } = null!;
    [CliData("name", "short")] public string ShouldSetNameAndShortName { get; set; } = null!;

    private CliDataAttribute GetAttribute(string nameOfProperty) {
        Type type = typeof(CliDataAttributeTests);
        PropertyInfo? propertyInfo = type.GetProperty(nameOfProperty);
        CliDataAttribute? attribute = propertyInfo.GetAttributes<CliDataAttribute>().FirstOrDefault();
        Fail.When(attribute is null, $"The attribute was not found for property {nameOfProperty}.");
        return attribute;
    }
    
    [Test]
    public async Task Should_set_name_and_short_name() {
        // Arrange
        
        // Act
        CliDataAttribute withoutData = GetAttribute(nameof(WithoutData));
        CliDataAttribute shouldSetName = GetAttribute(nameof(ShouldSetName));
        CliDataAttribute shouldSetShortName = GetAttribute(nameof(ShouldSetShortName));
        CliDataAttribute shouldSetNameAndShortName = GetAttribute(nameof(ShouldSetNameAndShortName));
        
        // Assert
        await Assert.That(withoutData)
            .HasMember(x => x.Name).EqualTo(null)
            .And.HasMember(x => x.ShortName).EqualTo(null);
        
        await Assert.That(shouldSetName)
            .HasMember(x => x.Name).EqualTo("name")
            .And.HasMember(x => x.ShortName).EqualTo(null);
        
        await Assert.That(shouldSetShortName)
            .HasMember(x => x.Name).EqualTo(null)
            .And.HasMember(x => x.ShortName).EqualTo("short");
        
        await Assert.That(shouldSetNameAndShortName)
            .HasMember(x => x.Name).EqualTo("name")
            .And.HasMember(x => x.ShortName).EqualTo("short");
    }
}
