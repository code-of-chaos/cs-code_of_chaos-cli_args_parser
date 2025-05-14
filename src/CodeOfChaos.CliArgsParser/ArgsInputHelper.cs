// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.RegularExpressions;

namespace CodeOfChaos.CliArgsParser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static partial class ArgsInputHelper {
    [GeneratedRegex("""=".*"$""", RegexOptions.Compiled)]
    private static partial Regex AlreadyQuotedPattern { get; }
    [GeneratedRegex("^([^=]+)=(.+)$", RegexOptions.Compiled)]
    private static partial Regex KvpPattern { get; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static string ToOneLine(IEnumerable<string> input) {
        ReadOnlySpan<string> inputSpan = input as string[] ?? input.ToArray();
        if (inputSpan.IsEmpty) return string.Empty;

        // Calculate total length to avoid reallocations
        int totalLength = CalculateTotalLength(inputSpan);

        return string.Create(totalLength, inputSpan.ToArray(), action: (span, args) => {
            int position = 0;

            for (int i = 0; i < args.Length; i++) {
                if (i > 0)
                    span[position++] = ' ';

                string arg = args[i];

                // If already properly quoted, copy as is
                if (AlreadyQuotedPattern.IsMatch(arg)) {
                    arg.CopyTo(span[position..]);
                    position += arg.Length;
                    continue;
                }

                // Handle key-value pairs
                Match kvpMatch = KvpPattern.Match(arg);
                if (kvpMatch.Success) {
                    string key = kvpMatch.Groups[1].Value;
                    string value = kvpMatch.Groups[2].Value;

                    key.CopyTo(span[position..]);
                    position += key.Length;
                    span[position++] = '=';
                    span[position++] = '"';
                    value.CopyTo(span[position..]);
                    position += value.Length;
                    span[position++] = '"';
                }
                // Handle space-containing arguments that aren't flags
                else if (arg.Contains(' ') && !arg.StartsWith('-')) {
                    span[position++] = '"';
                    arg.CopyTo(span[position..]);
                    position += arg.Length;
                    span[position++] = '"';
                }
                // Copy unchanged arguments
                else {
                    arg.CopyTo(span[position..]);
                    position += arg.Length;
                }
            }
        });
    }

    private static int CalculateTotalLength(ReadOnlySpan<string> input) {
        int length = 0;
        for (int i = 0; i < input.Length; i++) {
            if (i > 0) length++;// Space between arguments
            string arg = input[i];

            if (AlreadyQuotedPattern.IsMatch(arg)) {
                length += arg.Length;
            }
            else if (KvpPattern.IsMatch(arg) || arg.Contains(' ') && !arg.StartsWith('-')) {
                length += arg.Length + 2;// Add 2 for the quotes
            }
            else {
                length += arg.Length;
            }
        }

        return length;
    }


}
