namespace AwesomeAssertions.Common.Mismatch;

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
            MismatchSegment = GetMismatchSegment()
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

    private TextSegment GetMismatchSegment()
    {
        var subjectEntry = Context.SubjectSpan;
        var expectedEntry = Context.ExpectedSpan;

        int whiteSpaceCountBeforeArrow = Context.AdjustedIndexOfMismatch + Prefix.Length;

        if (subjectEntry.StartTruncated)
        {
            whiteSpaceCountBeforeArrow += 1;
        }

        var segment = new TextSegment
        {
            Lines =
            {
                new()
                {
                    Text = WrapText(subjectEntry)
                },
                new()
                {
                    Text = WrapText(expectedEntry)
                },
            }
        };

        if (Context.Reversed)
        {
            segment.AlignRight();
        }


        segment.Lines.Insert(0, new()
        {
            Text = $"{ArrowDown} (actual)",
            Indent = whiteSpaceCountBeforeArrow
        });
        segment.Lines.Add(new()
        {
            Text = $"{ArrowUp} (expected)",
            Indent = whiteSpaceCountBeforeArrow
        });

        return segment;
    }

    private static string WrapText(ITextSpan textSpan)
    {
        var innerPrefix = textSpan.StartTruncated ? Ellipsis : "";
        var innerSuffix = textSpan.EndTruncated ? Ellipsis : "";
        return $"{Prefix}{innerPrefix}{textSpan.VisibleText}{innerSuffix}{Suffix}";
    }
}
