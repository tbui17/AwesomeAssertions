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

        var revSubject = subject.Reversed();
        var revExpected = expected.Reversed();
        var mismatchIndex = revSubject.IndexOfFirstMismatch(revExpected, StringComparer.Ordinal);
        var factory = new TextSpanFactory(StringComparer.Ordinal);
        var subjectEntry = factory.CreateReversed(revSubject, mismatchIndex);
        var expectedEntry = factory.CreateReversed(revExpected, mismatchIndex);

        string mismatchSegment = GetMismatchSegment(subjectEntry, expectedEntry).EscapePlaceholders();

        return $$"""
            {{expectationDescription}}the expected string{reason}, but they differ {{locationDescription}}:
            {{mismatchSegment}}.
            """;
    }

    private static string GetMismatchSegment(IMismatchTextSpan subjectEntry, ITextSpan expectedEntry)
    {
        int whiteSpaceCountBeforeArrow = subjectEntry.MismatchIndex + Prefix.Length;

        if (subjectEntry.StartTruncated)
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

    private static StringBuilder AppendPrefixAndEscapedPhraseToShowWithEllipsisAndSuffix(StringBuilder stringBuilder, ITextSpan textSpan)
    {
        stringBuilder.Append(Prefix); // indent and opening quote

        if (textSpan.StartTruncated)
        {
            stringBuilder.Append(Ellipsis);
        }

        stringBuilder.Append(textSpan.VisibleText.Reversed());

        if (textSpan.EndTruncated)
        {
            stringBuilder.Append(Ellipsis);
        }

        stringBuilder.AppendLine(Suffix); // closing quote
        return stringBuilder;
    }
}
