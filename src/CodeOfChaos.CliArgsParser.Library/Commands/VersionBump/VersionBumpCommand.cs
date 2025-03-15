// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser.Library.Shared;
using System.Xml.Linq;

namespace CodeOfChaos.CliArgsParser.Library.Commands.VersionBump;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[CliArgsCommand("git-version-bump")]
[CliArgsDescription("Bumps the version of the projects specified in the projects argument.")]
public partial class VersionBumpCommand : ICommand<VersionBumpParameters> {
    private static readonly List<string> ErrorMessages = [];

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async Task ExecuteAsync(VersionBumpParameters parameters) {
        Console.WriteLine(ConsoleTextStore.BumpingVersion);
        SemanticVersionDto? updatedVersion = await BumpVersion(parameters);
        if (updatedVersion is null) {
            foreach (string message in ErrorMessages) Console.WriteLine(ConsoleTextStore.CommandEndFailure(message));
            return;
        }
        
        // Ask the user for extra input to make sure they want to commit and the current tag.
        if (!parameters.Force) {
            Console.WriteLine(ConsoleTextStore.QuestionTagAndCommit);
            char? input = Console.ReadLine()?.ToLowerInvariant().FirstOrDefault();
            if (input is not 'y') {
                Console.WriteLine(ConsoleTextStore.CommandEndSuccess());
                return;
            }
        }

        Console.WriteLine(ConsoleTextStore.GitCommitting);
        bool gitCommitResult = await GitHelpers.TryCreateGitCommit(updatedVersion);
        if (!gitCommitResult) {
            Console.WriteLine(ConsoleTextStore.CommandEndFailure("Git Committing failed"));
            return;
        }

        Console.WriteLine(ConsoleTextStore.GitTagging);
        bool gitTagResult = await GitHelpers.TryCreateGitTag(updatedVersion);
        if (!gitTagResult) {
            Console.WriteLine(ConsoleTextStore.CommandEndFailure("Git Tagging failed"));
            return;
        }

        Console.WriteLine(ConsoleTextStore.TagSuccessful(updatedVersion));

        if (!parameters.PushToRemote) {
            Console.WriteLine(ConsoleTextStore.CommandEndSuccess());
            return;
        }

        Console.WriteLine(ConsoleTextStore.GitPushingToRemote);
        bool pushResult = await GitHelpers.TryPushToOrigin();
        if (!pushResult) {
            Console.WriteLine(ConsoleTextStore.CommandEndFailure("Git Pushing failed"));
            return;
        }
        
        bool pushTagsResult = await GitHelpers.TryPushTagsToOrigin();
        if (!pushTagsResult) {
            Console.WriteLine(ConsoleTextStore.CommandEndFailure("Git Pushing Tags failed"));
            return;
        }

        Console.WriteLine(ConsoleTextStore.CommandEndSuccess());
    }


    private static async Task<SemanticVersionDto?> BumpVersion(VersionBumpParameters args) {
        string[] projectFiles = CsProjHelpers.AsProjectPaths(args.Root, args.SourceFolder, args.GetProjects());
        if (projectFiles.Length == 0) {
            ErrorMessages.Add("No projects specified");
            return null;
        }

        VersionSection sectionToBump = args.Section;
        SemanticVersionDto? versionDto = null;

        await foreach (XDocument document in CsProjHelpers.GetProjectFiles(projectFiles)) {
            XElement? versionElement = document
                .Descendants("PropertyGroup")
                .Elements("Version")
                .FirstOrDefault();

            // Only needed for logging, so setting to "UNKNOWN" is okay
            string projectName = document
                .Descendants("PropertyGroup")
                .Elements("PackageId")
                .FirstOrDefault()?
                .Value ?? "UNKNOWN";

            if (versionElement == null) {
                ErrorMessages.Add($"File {projectName} did not contain a version element");
                continue;
            }

            if (versionDto is null) {
                if (!SemanticVersionDto.TryParse(versionElement.Value, out SemanticVersionDto? dto)) {
                    ErrorMessages.Add($"File {projectName} contained an invalid version element: {versionElement.Value}");
                    continue;
                }

                dto.BumpVersion(sectionToBump);

                versionDto = dto;
            }

            versionElement.Value = versionDto.ToString();
            Console.WriteLine(ConsoleTextStore.UpdatedVersion(projectName, versionElement.Value));
        }

        return versionDto;
    }
}
