using System.Linq;

namespace AwesomeAssertions.Common.Mismatch;

internal class BaseMismatchTextSpan(ITextSpan textSpan, int mismatchIndex) : IMismatchTextSpan
{
    public int Start => textSpan.Start;

    public int End => textSpan.End;

    public int Length => End - Start;

    public int MismatchIndex => mismatchIndex - textSpan.Start;

    public string VisibleText => textSpan.VisibleText;

    public bool StartElided => textSpan.StartElided;

    public bool EndElided => textSpan.EndElided;
}

internal class MismatchTextSpan(BaseMismatchTextSpan textSpan) : IMismatchTextSpan
{
    public static MismatchTextSpan Create(string text, int mismatchIndex)
    {
        var factory = new TextSpanFactory();
        return factory.Create(text, mismatchIndex);
    }

    public int Start => GetRelativeOffset(textSpan.Start);

    public int End => GetRelativeOffset(textSpan.End - 1);

    public int Length => End - Start;

    public int MismatchIndex => GetRelativeOffset(textSpan.MismatchIndex);

    public string VisibleText => textSpan.VisibleText.EscapeNewLines();

    public bool StartElided => textSpan.StartElided;

    public bool EndElided => textSpan.EndElided;

    private int GetRelativeOffset(int index) => textSpan.VisibleText.Take(index + 1).Count(c => c is '\r' or '\n') + index;
}

internal class ReversedMismatchTextSpan(BaseMismatchTextSpan textSpan) : IMismatchTextSpan
{
    public int Start => textSpan.End;

    public int End => textSpan.Start;

    public string VisibleText => textSpan.VisibleText;

    public bool StartElided => textSpan.EndElided;

    public bool EndElided => textSpan.StartElided;

    public int Length => textSpan.Length;

    public int MismatchIndex => textSpan.MismatchIndex;
}
