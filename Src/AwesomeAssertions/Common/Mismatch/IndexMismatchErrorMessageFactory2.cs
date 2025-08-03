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
        var subjectEntry = factory.CreateReversed(revSubject, mismatchIndex);
        var expectedEntry = factory.CreateReversed(revExpected, mismatchIndex);

        int whiteSpaceCountBeforeArrow = subjectEntry.MismatchIndex + Prefix.Length;

        if (subjectEntry.StartElided)
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

    private static StringBuilder AppendPrefixAndEscapedPhraseToShowWithEllipsisAndSuffix(StringBuilder stringBuilder, IMismatchTextSpan textSpan)
    {
        stringBuilder.Append(Prefix); // indent and opening quote

        if (textSpan.StartElided)
        {
            stringBuilder.Append(Ellipsis);
        }

        stringBuilder.Append(textSpan.VisibleText);

        if (textSpan.EndElided)
        {
            stringBuilder.Append(Ellipsis);
        }

        stringBuilder.AppendLine(Suffix); // closing quote
        return stringBuilder;
    }
}
