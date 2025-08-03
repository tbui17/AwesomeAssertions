using System.Linq;

namespace AwesomeAssertions.Common.Mismatch;

internal class MismatchContext
{
    public required string Subject { get; init; }

    public required string Expected { get; init; }

    public required ITextSpan SubjectSpan { get; init; }

    public required ITextSpan ExpectedSpan { get; init; }

    public required int AdjustedIndexOfMismatch { get; init; }

    public required int IndexOfMismatch { get; init; }

    public required bool Reversed { get; init; }

    private string MismatchString => Subject[..IndexOfMismatch];

    public int GetMismatchLineNumber() => MismatchString.Count(c => c == '\n') + 1;

    public int GetMismatchColumn()
    {
        var indexOfLastNewlineBeforeMismatch = MismatchString.LastIndexOf('\n');
        return MismatchString.Length - indexOfLastNewlineBeforeMismatch;
    }
}

internal class IndexMismatchErrorMessageFactory
{
    private const string Indentation = "  ";
    private const string Prefix = Indentation + "\"";
    private const string Suffix = "\"";
    private const string ArrowDown = "\u2193";
    private const string ArrowUp = "\u2191";
    private const string Ellipsis = "\u2026";

    public required MismatchContext Context { get; init; }

    public required string ExpectationDescription { get; init; }

    public IndexMismatchErrorMessage Create()
    {
        return new IndexMismatchErrorMessage
        {
            ExpectationDescription = ExpectationDescription,
            LocationDescription = CreateLocationDescription(),
            MismatchSegment = GetMismatchSegmentImpl()
        };
    }

    private string CreateLocationDescription()
    {
        var lineNumber = Context.GetMismatchLineNumber();
        var indexOfMismatch = Context.IndexOfMismatch;
        if (lineNumber > 1)
        {
            var column = Context.GetMismatchColumn();
            return $"on line {lineNumber} and column {column} (index {indexOfMismatch})";
        }

        var prefix = Context.Reversed ? "before" : "at";
        return $"{prefix} index {indexOfMismatch}";
    }

    private TextSegment GetMismatchSegmentImpl()
    {
        var subjectEntry = Context.SubjectSpan;
        var expectedEntry = Context.ExpectedSpan;

        int whiteSpaceCountBeforeArrow = Context.AdjustedIndexOfMismatch + Prefix.Length;

        if (subjectEntry.StartTruncated)
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
        var innerPrefix = textSpan.StartTruncated ? Ellipsis : "";
        var innerSuffix = textSpan.EndTruncated ? Ellipsis : "";
        return $"{Prefix}{innerPrefix}{textSpan.VisibleText}{innerSuffix}{Suffix}";
    }
}
