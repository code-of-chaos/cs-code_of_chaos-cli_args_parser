// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser;
using CodeOfChaos.CliArgsParser.Library;

namespace Tools.CodeOfChaos.CliArgsParser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class Program {
    public static async Task Main(string[] args) {
        // Register & Build the parser
        //      Don't forget to add the current assembly if you built more tools for the current project
        ICliParser parser = CliParser.FromBuilder()
            .AddCommandsFromAssembly<IAssemblyEntry>()
            .Build();

        // We are doing this here because else the launchSettings.json file becomes a humongous issue to deal with.
        //      Sometimes CLI params are not the answer.
        //      Code is the true savior
        // const string projects = "";
        string projects = string.Join(";",
            "CodeOfChaos.CliArgsParser",
            "CodeOfChaos.CliArgsParser.Generators",
            "CodeOfChaos.CliArgsParser.Library",
            "CodeOfChaos.CliArgsParser.Contracts"
        );

        string oneLineArgs = ArgsInputHelper.ToOneLine(args).Replace("%PROJECTS%", projects);

        // Finally, start executing
        await parser.ExecuteAsync(oneLineArgs);
    }
}
