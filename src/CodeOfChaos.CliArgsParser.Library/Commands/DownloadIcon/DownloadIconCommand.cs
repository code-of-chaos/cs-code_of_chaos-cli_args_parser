// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser.Library.Shared;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace CodeOfChaos.CliArgsParser.Library.Commands.DownloadIcon;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[CliArgsCommand("nuget-download-icon")]
[CliArgsDescription("Downloads and assigns the icon, to be used as nuget package's icon, for the specified project.")]
public partial class DownloadIconCommand : ICommand<DownloadIconParameters> {

    [GeneratedRegex(@"^[^/\\\s]+$")]
    private static partial Regex IsEmptyFolderNameRegex { get; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async Task ExecuteAsync(DownloadIconParameters parameters) {
        Console.WriteLine("Downloading Icon...");
        bool getResult = await TryGetIcon(parameters);
        if (!getResult) {
            Console.WriteLine(ConsoleTextStore.CommandEndFailure("Could not download icon."));
            return;
        }

        Console.WriteLine("Icon downloaded successfully.");
        bool applyResult = await ApplyIcon(parameters);
        if (!applyResult) {
            Console.WriteLine(ConsoleTextStore.CommandEndFailure("Could not apply icon."));
            return;
        }

        Console.WriteLine("Icon applied successfully.");
    }

    private static async Task<bool> ApplyIcon(DownloadIconParameters args) {
        string[] projectFiles = CsProjHelpers.AsProjectPaths(args.Root, args.SourceFolder, args.GetProjects());
        if (projectFiles.Length == 0) {
            Console.WriteLine(ConsoleTextStore.CommandEndFailure("No projects specified"));
            return false;
        }

        await foreach (XDocument document in CsProjHelpers.GetProjectFiles(projectFiles)) {

            // Loop through each project file's XML document
            foreach (XElement propertyGroup in document.Root?.Elements("PropertyGroup")!) {
                XElement? iconElement = propertyGroup.Element("PackageIcon");

                // If <PackageIcon> does not exist, create it
                if (iconElement is null) {
                    iconElement = new XElement("PackageIcon", "icon.png");
                    propertyGroup.Add(iconElement);
                }
                else {
                    // Update the value to ensure it's "icon.png"
                    iconElement.Value = "icon.png";
                }

                // Look for ItemGroup containing Packable items
                XElement? packableItemGroup = document.Root?
                    .Elements("ItemGroup")
                    .FirstOrDefault(group => group.Elements("None")
                        .Any(item => item.Attribute("Pack")?.Value == "true"));

                if (packableItemGroup is null) {
                    // Create the ItemGroup if it doesn't exist
                    packableItemGroup = new XElement("ItemGroup");
                    document.Root?.Add(packableItemGroup);
                }

                // Check if the icon.png item already exists
                bool iconExists = packableItemGroup.Elements("None")
                    .Any(item => item.Attribute("Include")?.Value.EndsWith("icon.png") == true);

                if (iconExists) continue;

                IEnumerable<string> indents = Enumerable.Repeat(
                    "../",
                    1
                    + (IsEmptyFolderNameRegex.IsMatch(args.SourceFolder) ? 1 : 0)
                    + args.SourceFolder.Count(c => c is '/' or '\\')
                );

                string includeString = Path.Combine(string.Join(string.Empty, indents), args.IconFolder, "icon.png");

                // Add the icon.png reference if it doesn't exist
                var newIconElement = new XElement("None",
                    new XAttribute("Include", includeString),
                    new XAttribute("Pack", "true"),
                    new XAttribute("PackagePath", ""));

                packableItemGroup.Add(newIconElement);
            }
        }

        return true;
    }

    private static async Task<bool> TryGetIcon(DownloadIconParameters parameters) {
        
            // Validate Origin
            if (string.IsNullOrWhiteSpace(parameters.Origin)) {
                Console.WriteLine(ConsoleTextStore.CommandEndFailure( "Error: The origin of the icon is not specified."));
                return false;
            }

            // Assume Origin could be either a URL or a file path
            bool isUrl = Uri.TryCreate(parameters.Origin, UriKind.Absolute, out Uri? uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            // Placeholder for the final icon's path
            string destinationPath = Path.Combine(parameters.Root, parameters.IconFolder, "icon.png");

            // create all folders if needed for the destination path
            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);

            if (isUrl) {
                // Download the file from URL using HttpClient
                using var client = new HttpClient();
                Console.WriteLine($"Downloading icon from URL: {parameters.Origin}");

                using HttpResponseMessage response = await client.GetAsync(parameters.Origin);
                if (!response.IsSuccessStatusCode) {
                    Console.WriteLine(ConsoleTextStore.CommandEndFailure($"Error: Failed to download the icon. HTTP Status: {response.StatusCode}"));
                    return false;
                }

                byte[] iconBytes = await response.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync(destinationPath, iconBytes);

                Console.WriteLine($"Icon downloaded successfully to: {destinationPath}");
            }
            else {
                // Treat as file system path and copy the icon
                Console.WriteLine($"Copying icon from local path: {parameters.Origin}");

                if (!File.Exists(parameters.Origin)) {
                    Console.WriteLine(ConsoleTextStore.CommandEndFailure($"Error: The specified origin file does not exist: {parameters.Origin}"));
                    return false;
                }

                File.Copy(parameters.Origin, destinationPath, true);
                Console.WriteLine($"Icon copied successfully to: {destinationPath}");
            }

            // If all operations succeed
            return true;
    }
}
