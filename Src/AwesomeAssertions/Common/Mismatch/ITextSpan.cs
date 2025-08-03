using System.Linq;

namespace AwesomeAssertions.Common.Mismatch;

internal interface ITextSpan
{
    int Start { get; }

    int End { get; }

    string VisibleText { get; }

    bool StartTruncated { get; }

    bool EndTruncated { get; }

    int Length { get; }
}

internal interface IMismatchTextSpan : ITextSpan
{
    int MismatchIndex { get; }
}

internal static class TextSpanExtensions
{
    public static int GetRelativeOffset(this ITextSpan textSpan, int index) => textSpan.VisibleText.Take(index + 1).Count(c => c is '\r' or '\n') + index;
}
