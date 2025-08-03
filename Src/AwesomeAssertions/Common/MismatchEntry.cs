using System.Linq;

namespace AwesomeAssertions.Common;

internal class MismatchEntry(TextSpan textSpan, int mismatchIndex)
{
    public static MismatchEntry Create(string text, int mismatchIndex)
    {
        var factory = new MismatchEntryFactory();
        return factory.Create(text, mismatchIndex);
    }

    public TextSpan TextSpan => textSpan;

    public int Start => GetRelativeOffset(textSpan.Start);

    public int End => GetRelativeOffset(textSpan.End - 1);

    public int Length => End - Start;

    private int RelativeMismatchIndex => mismatchIndex - textSpan.Start;

    public int MismatchIndex => GetRelativeOffset(RelativeMismatchIndex);

    public string VisibleText => textSpan.VisibleText.EscapeNewLines();

    private int GetRelativeOffset(int index) => textSpan.VisibleText.Take(index + 1).Count(c => c is '\r' or '\n') + index;
}
