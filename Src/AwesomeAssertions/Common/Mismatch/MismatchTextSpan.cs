using System;
using System.Linq;

namespace AwesomeAssertions.Common.Mismatch;

internal class MismatchTextSpan(ITextSpan textSpan, int mismatchIndexValue = 0) : IMismatchTextSpan
{
    public int MismatchIndexValue { get; set; } = mismatchIndexValue;

    public int Start => textSpan.Start;

    public int End => textSpan.End;

    public int Length => textSpan.Length;

    public int MismatchIndex => MismatchIndexValue - textSpan.Start;

    public string VisibleText => textSpan.VisibleText;

    public bool StartTruncated => textSpan.StartTruncated;

    public bool EndTruncated => textSpan.EndTruncated;
}

internal class EscapeNewLinesMismatchTextSpanDecorator(ITextSpan textSpan) : IMismatchTextSpan
{
    public int Start => throw new NotImplementedException();

    public int End => throw new NotImplementedException();

    public int Length => throw new NotImplementedException();

    public int MismatchIndex
    {
        get
        {
            if (textSpan is IMismatchTextSpan mismatchTextSpan)
            {
                return GetRelativeOffset(mismatchTextSpan.MismatchIndex);
            }
            throw new NotImplementedException();
        }
    }

    public string VisibleText => textSpan.VisibleText.EscapeNewLines();

    public bool StartTruncated => textSpan.StartTruncated;

    public bool EndTruncated => textSpan.EndTruncated;

    private int GetRelativeOffset(int index) => textSpan.VisibleText.Take(index + 1).Count(c => c is '\r' or '\n') + index;
}

internal class ReverseMismatchTextSpan(IMismatchTextSpan textSpan) : IMismatchTextSpan
{
    public int Start => MirrorIndex(textSpan.Length, textSpan.End);

    public int End => MirrorIndex(textSpan.Length, textSpan.Start);

    public string VisibleText => textSpan.VisibleText.Reversed();

    public bool StartTruncated => textSpan.EndTruncated;

    public bool EndTruncated => textSpan.StartTruncated;

    public int Length => textSpan.Length;

    public int MismatchIndex => MirrorIndex(textSpan.Length, textSpan.MismatchIndex);

    private static int MirrorIndex(int stringLength, int originalIndex) => stringLength - originalIndex - 1;
}
