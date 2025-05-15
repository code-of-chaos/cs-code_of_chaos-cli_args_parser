// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace CodeOfChaos.CliArgsParser.Library.Commands.VersionBump;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public record VersionBumpParameters : ICliParameters {
    [CliData("root", "r")] 
    // [CliArgsDescription("The root directory of the project to update")]
    public string Root { get; init; } = "../../../../../";

    [CliData("section", "s")] 
    // [CliArgsDescription("The section of the version to bump. One of: Major, Minor, Patch")]
    public required string? SectionStringValue { get; init; }

    public VersionSection Section => Enum.Parse<VersionSection>(SectionStringValue ?? "None", true);

    [CliData("push", "p")] 
    // [CliArgsDescription("Push the changes to the remote repository")]
    public bool PushToRemote { get; init; } = false;

    [CliData("force", "f")] 
    // [CliArgsDescription("Automatically push the changes to the remote repository without user input")]
    public bool Force { get; init; } = false;

    [CliData("projects", "pr")] 
    // [CliArgsDescription("The projects to update")]
    public required string ProjectsStringValue { get; init; }

    [CliData("project-split", "ps")] 
    // [CliArgsDescription("The split character to use when splitting the projects string")]
    public string ProjectsSplit { get; init; } = ";";

    [CliData("source-folder", "sf")] 
    // [CliArgsDescription("The folder where the projects are located")]
    public string SourceFolder { get; init; } = "src";

    public string[] GetProjects() => ProjectsStringValue
        .Split(ProjectsSplit)
        .Where(entry => !string.IsNullOrWhiteSpace(entry))
        .ToArray();

}
