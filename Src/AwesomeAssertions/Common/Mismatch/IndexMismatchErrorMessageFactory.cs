using System.Linq;

namespace AwesomeAssertions.Common.Mismatch;

internal static class IndexMismatchErrorMessageFactory
{
    private const string Indentation = "  ";
    private const string Prefix = Indentation + "\"";
    private const string Suffix = "\"";
    private const string ArrowDown = "\u2193";
    private const string ArrowUp = "\u2191";
    private const string Ellipsis = "\u2026";

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
        var factory = new TextSpanFactory();
        var subjectEntry = factory.Create(subject, firstIndexOfMismatch);
        var expectedEntry = factory.Create(expected, firstIndexOfMismatch);



        int whiteSpaceCountBeforeArrow = subjectEntry.MismatchIndex + Prefix.Length;
        if (subjectEntry.StartElided)
        {
            whiteSpaceCountBeforeArrow += 1;
        }

        return new TextSegment
        {
            Lines =
            {
                new()
                {
                    Text = $"{ArrowDown} (actual)",
                    Indent = whiteSpaceCountBeforeArrow
                },
                new()
                {
                    Text = WrapText(subjectEntry)
                },
                new()
                {
                    Text = WrapText(expectedEntry)
                },
                new()
                {
                    Text = $"{ArrowUp} (expected)",
                    Indent = whiteSpaceCountBeforeArrow
                }
            }
        };
    }

    private static string WrapText(ITextSpan textSpan)
    {
        var innerPrefix = textSpan.StartElided ? Ellipsis : "";
        var innerSuffix = textSpan.EndElided ? Ellipsis : "";
        return $"{Prefix}{innerPrefix}{textSpan.VisibleText}{innerSuffix}{Suffix}";
    }
}
