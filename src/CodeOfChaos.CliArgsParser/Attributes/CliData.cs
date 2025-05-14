// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace CodeOfChaos.CliArgsParser;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
public class CliDataAttribute(string? name = null, string? shortName = null) : Attribute {
    public string? Name { get; } = name;
    public string? ShortName { get; } = shortName;
}
