// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.Ansi;
using System.Diagnostics;

namespace CodeOfChaos.CliArgsParser.Library.Shared;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class GitHelpers {
    public static async Task<bool> TryPushToOrigin() {
        var gitTagInfo = new ProcessStartInfo("git", "push origin") {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var builder = new AnsiStringBuilder();

        using Process? gitTagProcess = Process.Start(gitTagInfo);

        builder.Fore.AppendWhitesmokeLine(await gitTagProcess?.StandardOutput.ReadToEndAsync()!);
        Console.WriteLine(builder.ToStringAndClear());

        await gitTagProcess.WaitForExitAsync();

        return gitTagProcess.ExitCode == 0;
    }


    public static async Task<bool> TryPushTagsToOrigin() {
        var gitTagInfo = new ProcessStartInfo("git", "push origin --tags") {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var builder = new AnsiStringBuilder();

        using Process? gitTagProcess = Process.Start(gitTagInfo);

        builder.Fore.AppendWhitesmokeLine(await gitTagProcess?.StandardOutput.ReadToEndAsync()!);
        Console.WriteLine(builder.ToStringAndClear());

        await gitTagProcess.WaitForExitAsync();

        return gitTagProcess.ExitCode == 0;
    }


    public static async Task<bool> TryCreateGitTag(SemanticVersionDto updatedVersion) {
        var gitTagInfo = new ProcessStartInfo("git", "tag v" + updatedVersion) {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var builder = new AnsiStringBuilder();

        using Process? gitTagProcess = Process.Start(gitTagInfo);

        builder.Fore.AppendWhitesmokeLine(await gitTagProcess?.StandardOutput.ReadToEndAsync()!);
        Console.WriteLine(builder.ToStringAndClear());

        await gitTagProcess.WaitForExitAsync();

        return gitTagProcess.ExitCode == 0;
    }

    public static async Task<bool> TryCreateGitCommit(SemanticVersionDto updatedVersion) {
        var gitCommitInfo = new ProcessStartInfo("git", $"commit -am \"VersionBump : v{updatedVersion}\"") {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var builder = new AnsiStringBuilder();

        using Process? gitCommitProcess = Process.Start(gitCommitInfo);

        builder.Fore.AppendWhitesmokeLine(await gitCommitProcess?.StandardOutput.ReadToEndAsync()!);
        Console.WriteLine(builder.ToStringAndClear());

        await gitCommitProcess.WaitForExitAsync();

        return gitCommitProcess.ExitCode == 0;

    }
}
