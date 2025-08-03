using System.Linq;

namespace AwesomeAssertions.Common.Mismatch;

internal class MismatchEntry(TextSpan textSpan, int mismatchIndex) : ITextSpan
{
    public static MismatchEntry Create(string text, int mismatchIndex)
    {
        var factory = new TextSegmentFactory();
        return factory.Create(text, mismatchIndex);
    }

    public int Start => GetRelativeOffset(textSpan.Start);

    public int End => GetRelativeOffset(textSpan.End - 1);

    public int Length => End - Start;

    private int RelativeMismatchIndex => mismatchIndex - textSpan.Start;

    public int MismatchIndex => GetRelativeOffset(RelativeMismatchIndex);

    public string VisibleText => textSpan.VisibleText.EscapeNewLines();

    public bool StartElided => textSpan.StartElided;

    public bool EndElided => textSpan.EndElided;

    private int GetRelativeOffset(int index) => textSpan.VisibleText.Take(index + 1).Count(c => c is '\r' or '\n') + index;
}
