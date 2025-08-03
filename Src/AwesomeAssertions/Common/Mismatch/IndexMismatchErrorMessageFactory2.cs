using System;
using System.Linq;
using System.Text;

namespace AwesomeAssertions.Common.Mismatch;

internal static class IndexMismatchErrorMessageFactory2
{
    private const string Indentation = "  ";
    private const string Prefix = Indentation + "\"";
    private const string Suffix = "\"";
    private const char ArrowDown = '\u2193';
    private const char ArrowUp = '\u2191';
    private const char Ellipsis = '\u2026';

    public static string CreateFailureMessage(string expectationDescription, string subject, string expected, int indexOfMismatch)
    {
        string locationDescription = $"before index {indexOfMismatch}";
        var matchingString = subject[..indexOfMismatch];
        int lineNumber = matchingString.Count(c => c == '\n');

        if (lineNumber > 0)
        {
            var indexOfLastNewlineBeforeMismatch = matchingString.LastIndexOf('\n');
            var column = matchingString.Length - indexOfLastNewlineBeforeMismatch;
            locationDescription = $"on line {lineNumber + 1} and column {column} (index {indexOfMismatch})";
        }

        string mismatchSegment = GetMismatchSegment(subject, expected, indexOfMismatch).EscapePlaceholders();

        return $$"""
            {{expectationDescription}}the expected string{reason}, but they differ {{locationDescription}}:
            {{mismatchSegment}}.
            """;
    }

    /// <summary>
    /// Get the mismatch segment between <paramref name="expected"/> and <paramref name="subject"/>,
    /// when they differ at index <paramref name="firstIndexOfMismatch"/>.
    /// </summary>
    private static string GetMismatchSegment(string subject, string expected, int firstIndexOfMismatch)
    {
        var revSubject = subject.Reversed();
        var revExpected = expected.Reversed();
        var mismatchIndex = revSubject.IndexOfFirstMismatch(revExpected, StringComparer.Ordinal);
        var factory = new TextSpanFactory();
        var subjectEntry = factory.CreateBase(revSubject, mismatchIndex);
        var expectedEntry = factory.CreateBase(revExpected, mismatchIndex);

        int whiteSpaceCountBeforeArrow = ReversedTextSpan.GetIndexAfterReversal(subjectEntry.Length, subjectEntry.MismatchIndex) + Prefix.Length;

        if (subjectEntry.EndElided)
        {
            whiteSpaceCountBeforeArrow++;
        }

        var sb = new StringBuilder();

        sb.Append(' ', whiteSpaceCountBeforeArrow).Append(ArrowDown).AppendLine(" (actual)");
        var res1 = AppendPrefixAndEscapedPhraseToShowWithEllipsisAndSuffix(new StringBuilder(), subjectEntry);
        var res2 = AppendPrefixAndEscapedPhraseToShowWithEllipsisAndSuffix(new StringBuilder(), expectedEntry);
        res2 = res2.PadLeft(res1.Length);
        sb.Append(res1);
        sb.Append(res2);
        sb.Append(' ', whiteSpaceCountBeforeArrow).Append(ArrowUp).Append(" (expected)"); // expected arrow must always have same column as actual arrow

        return sb.ToString();
    }

    private static string AppendPrefixAndEscapedPhraseToShowWithEllipsisAndSuffix(StringBuilder stringBuilder, IMismatchTextSpan textSpan)
    {
        stringBuilder.Append(Prefix); // indent and opening quote

        if (textSpan.EndElided)
        {
            stringBuilder.Append(Ellipsis);
        }

        stringBuilder.Append(textSpan.VisibleText.Reverse().Join().EscapeNewLines());

        if (textSpan.StartElided)
        {
            stringBuilder.Append(Ellipsis);
        }

        stringBuilder.AppendLine(Suffix); // closing quote
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Calculates the start index of the visible segment from <paramref name="value"/> when highlighting the difference at <paramref name="indexOfFirstMismatch"/>.
    /// </summary>
    /// <remarks>
    /// Either keep the last 10 characters before <paramref name="indexOfFirstMismatch"/> or a word begin (separated by whitespace) between 15 and 5 characters before <paramref name="indexOfFirstMismatch"/>.
    /// </remarks>
    private static int GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex(string value, int indexOfFirstMismatch)
    {
        const int defaultCharactersToKeep = 10;
        const int minCharactersToKeep = 5;
        const int maxCharactersToKeep = 15;
        const int lengthOfWhitespace = 1;
        const int phraseLengthToCheckForWordBoundary = (maxCharactersToKeep - minCharactersToKeep) + lengthOfWhitespace;

        if (indexOfFirstMismatch <= defaultCharactersToKeep)
        {
            return 0;
        }

        var indexToStartSearchingForWordBoundary = Math.Max(indexOfFirstMismatch - (maxCharactersToKeep + lengthOfWhitespace), 0);

        var indexOfWordBoundary = value
                .IndexOf(' ', indexToStartSearchingForWordBoundary, phraseLengthToCheckForWordBoundary) -
            indexToStartSearchingForWordBoundary;

        if (indexOfWordBoundary >= 0)
        {
            return indexToStartSearchingForWordBoundary + indexOfWordBoundary + lengthOfWhitespace;
        }

        return indexOfFirstMismatch - defaultCharactersToKeep;
    }

    /// <summary>
    /// Calculates how many characters to keep in <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// If a word end is found between 45 and 60 characters, use this word end, otherwise keep 50 characters.
    /// </remarks>
    private static int GetLengthOfPhraseToShowOrDefaultLength(string value)
    {
        var defaultLength = AssertionConfiguration.Current.Formatting.StringPrintLength;
        int minLength = defaultLength - 5;
        int maxLength = defaultLength + 10;
        const int lengthOfWhitespace = 1;

        var indexOfWordBoundary = value
            .LastIndexOf(' ', Math.Min(maxLength + lengthOfWhitespace, value.Length) - 1);

        if (indexOfWordBoundary >= minLength)
        {
            return indexOfWordBoundary;
        }

        return Math.Min(defaultLength, value.Length);
    }
}
