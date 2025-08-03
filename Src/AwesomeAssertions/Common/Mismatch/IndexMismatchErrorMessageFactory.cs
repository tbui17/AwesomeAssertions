using System.Linq;
using System.Text;

namespace AwesomeAssertions.Common.Mismatch;

internal static class IndexMismatchErrorMessageFactory
{
    private const string Indentation = "  ";
    private const string Prefix = Indentation + "\"";
    private const string Suffix = "\"";
    private const char ArrowDown = '\u2193';
    private const char ArrowUp = '\u2191';
    private const char Ellipsis = '\u2026';

    public static IndexMismatchErrorMessage CreateFailureMessage(string expectationDescription, string subject, string expected, int indexOfMismatch)
    {
        string locationDescription = $"at index {indexOfMismatch}";
        var matchingString = subject[..indexOfMismatch];
        int lineNumber = matchingString.Count(c => c == '\n');

        if (lineNumber > 0)
        {
            var indexOfLastNewlineBeforeMismatch = matchingString.LastIndexOf('\n');
            var column = matchingString.Length - indexOfLastNewlineBeforeMismatch;
            locationDescription = $"on line {lineNumber + 1} and column {column} (index {indexOfMismatch})";
        }

        string mismatchSegment = GetMismatchSegment(subject, expected, indexOfMismatch).EscapePlaceholders();

        return new IndexMismatchErrorMessage
        {
            ExpectationDescription = expectationDescription,
            LocationDescription = locationDescription,
            MismatchSegment = mismatchSegment,
        };
    }

    /// <summary>
    /// Get the mismatch segment between <paramref name="expected"/> and <paramref name="subject"/>,
    /// when they differ at index <paramref name="firstIndexOfMismatch"/>.
    /// </summary>
    private static string GetMismatchSegment(string subject, string expected, int firstIndexOfMismatch)
    {
        var subjectEntry = MismatchEntry.Create(subject, firstIndexOfMismatch);
        var expectedEntry = MismatchEntry.Create(expected, firstIndexOfMismatch);



        int whiteSpaceCountBeforeArrow = subjectEntry.MismatchIndex + Prefix.Length;
        if (subjectEntry.TextSpan.StartElided)
        {
            whiteSpaceCountBeforeArrow += 1;
        }

        var sb = new StringBuilder();

        sb.Append(' ', whiteSpaceCountBeforeArrow).Append(ArrowDown).AppendLine(" (actual)");
        AppendPrefixAndEscapedPhraseToShowWithEllipsisAndSuffix(sb, subjectEntry);
        AppendPrefixAndEscapedPhraseToShowWithEllipsisAndSuffix(sb, expectedEntry);
        sb.Append(' ', whiteSpaceCountBeforeArrow).Append(ArrowUp).Append(" (expected)");

        return sb.ToString();
    }

    private static void AppendPrefixAndEscapedPhraseToShowWithEllipsisAndSuffix(StringBuilder stringBuilder, MismatchEntry entry)
    {
        stringBuilder.Append(Prefix); // indent and opening quote

        if (entry.TextSpan.StartElided)
        {
            stringBuilder.Append(Ellipsis);
        }

        stringBuilder.Append(entry.VisibleText);

        if (entry.TextSpan.EndElided)
        {
            stringBuilder.Append(Ellipsis);
        }

        stringBuilder.AppendLine(Suffix); // closing quote
    }
}
