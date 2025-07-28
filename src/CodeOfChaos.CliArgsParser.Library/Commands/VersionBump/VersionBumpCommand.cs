// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser.Library.Shared;
using System.Xml.Linq;

namespace CodeOfChaos.CliArgsParser.Library.Commands.VersionBump;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[CliData("git-version-bump")]
// [CliArgsDescription("Bumps the version of the projects specified in the project argument.")]
public partial class VersionBumpCommand : ICliCommand<VersionBumpParameters> {
    private static readonly List<string> ErrorMessages = [];

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async ValueTask ExecuteAsync(VersionBumpParameters parameters, CancellationToken ct = default) {
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

        // Process csproj files
        await foreach (XDocument document in CsProjHelpers.GetProjectFiles(projectFiles)) {
            string projectName = GetProjectNameFromCsproj(document);
            
            SemanticVersionDto? result = ProcessCsprojFile(document, projectName, versionDto, sectionToBump);
            if (result is not null) {
                versionDto = result;
            }
        }

        // Process nuspec files
        await ProcessNuspecFiles(projectFiles, versionDto);

        return versionDto;
    }

    private static SemanticVersionDto? ProcessCsprojFile(XDocument document, string projectName, SemanticVersionDto? currentVersion, VersionSection sectionToBump) {
        XElement? versionElement = document
            .Descendants("PropertyGroup")
            .Elements("Version")
            .FirstOrDefault();

        if (versionElement == null) {
            ErrorMessages.Add($"File {projectName} did not contain a version element");
            return null;
        }

        SemanticVersionDto? versionDto = currentVersion;
        
        if (versionDto is null) {
            if (!SemanticVersionDto.TryParse(versionElement.Value, out SemanticVersionDto? dto)) {
                ErrorMessages.Add($"File {projectName} contained an invalid version element: {versionElement.Value}");
                return null;
            }

            dto.BumpVersion(sectionToBump);
            versionDto = dto;
        }

        versionElement.Value = versionDto.ToString();
        Console.WriteLine(ConsoleTextStore.UpdatedVersion(projectName, versionElement.Value));
        return versionDto;
    }

    private static async Task ProcessNuspecFiles(string[] projectFiles, SemanticVersionDto? versionDto) {
        if (versionDto == null) return;

        foreach (string projectFile in projectFiles) {
            string nuspecFile = Path.ChangeExtension(projectFile, ".nuspec");
            if (!File.Exists(nuspecFile)) continue;

            try {
                XDocument nuspecDocument;
                await using (var stream = new FileStream(nuspecFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true)) {
                    nuspecDocument = await XDocument.LoadAsync(stream, LoadOptions.PreserveWhitespace, CancellationToken.None);
                }
                
                XNamespace ns = nuspecDocument.Root?.GetDefaultNamespace() ?? XNamespace.None;
                XElement? versionElement = nuspecDocument
                    .Descendants(ns + "metadata")
                    .Elements(ns + "version")
                    .FirstOrDefault();

                if (versionElement != null) {
                    versionElement.Value = versionDto.ToString();
                    
                    // Save the nuspec file
                    await using var stream = new FileStream(nuspecFile, FileMode.Create, FileAccess.Write);
                    await nuspecDocument.SaveAsync(stream, SaveOptions.None, CancellationToken.None);
                    
                    string nuspecName = Path.GetFileNameWithoutExtension(nuspecFile);
                    Console.WriteLine(ConsoleTextStore.UpdatedVersion(nuspecName, versionElement.Value));
                }
            }
            catch (Exception ex) {
                ErrorMessages.Add($"Failed to process nuspec file {nuspecFile}: {ex.Message}");
            }
        }
    }

    private static string GetProjectNameFromCsproj(XDocument document) {
        return document
            .Descendants("PropertyGroup")
            .Elements("PackageId")
            .FirstOrDefault()?
            .Value ?? "UNKNOWN";
    }

}
