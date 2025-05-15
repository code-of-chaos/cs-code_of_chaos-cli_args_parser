// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace CodeOfChaos.CliArgsParser.Generators.Helpers;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class StringCaseExtensions {
    private static readonly Regex NonAlphanumericRegex = new("(?<=[a-z])(?=[A-Z0-9])|(?<=[0-9])(?=[a-zA-Z])|[^a-zA-Z0-9]+", RegexOptions.Compiled);

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    [SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
    public static string ToKebabCase(this string input) {
        if (input.IsNullOrWhiteSpace()) return input;

        ReadOnlySpan<string> words = NonAlphanumericRegex.Split(input);

        Span<char> result = stackalloc char[input.Length * 2];// Overallocate to accommodate separators
        int position = 0;

        for (int i = 0; i < words.Length; i++) {
            if (words[i].IsNullOrEmpty()) continue;

            ReadOnlySpan<char> wordSpan = words[i].AsSpan();

            // Add separator for kebab-case
            if (position > 0) result[position++] = '-';

            // Append the word in lowercase
            for (int j = 0; j < wordSpan.Length; j++) {
                result[position++] = char.ToLower(wordSpan[j]);
            }
        }

        return new string(result[..position].ToArray());
    }
}
