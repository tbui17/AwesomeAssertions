using System.Linq;

namespace AwesomeAssertions.Common.Mismatch;

internal class MismatchTextSpan(ITextSpan textSpan, int mismatchIndex) : IMismatchTextSpan
{
    public int Start => textSpan.Start;

    public int End => textSpan.End;

    public int Length => End - Start;

    public int MismatchIndex => mismatchIndex - textSpan.Start;

    public string VisibleText => textSpan.VisibleText;

    public bool StartElided => textSpan.StartElided;

    public bool EndElided => textSpan.EndElided;
}

internal class EscapeNewLinesMismatchTextSpanDecorator(IMismatchTextSpan textSpan) : IMismatchTextSpan
{
    public int Start => GetRelativeOffset(textSpan.Start);

    public int End => GetRelativeOffset(textSpan.End - 1);

    public int Length => End - Start;

    public int MismatchIndex => GetRelativeOffset(textSpan.MismatchIndex);

    public string VisibleText => textSpan.VisibleText.EscapeNewLines();

    public bool StartElided => textSpan.StartElided;

    public bool EndElided => textSpan.EndElided;

    private int GetRelativeOffset(int index) => textSpan.VisibleText.Take(index + 1).Count(c => c is '\r' or '\n') + index;
}

internal class ReverseMismatchTextSpanDecorator(IMismatchTextSpan textSpan) : IMismatchTextSpan
{
    public int Start => textSpan.End;

    public int End => textSpan.Start;

    public string VisibleText => textSpan.VisibleText;

    public bool StartElided => textSpan.EndElided;

    public bool EndElided => textSpan.StartElided;

    public int Length => textSpan.Length;

    public int MismatchIndex => textSpan.MismatchIndex;
}
