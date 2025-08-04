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
