namespace AwesomeAssertions.Common;

public interface ITextSpan
{
    int Start { get; }

    int End { get; }

    string Text { get; }

    int MismatchIndex { get; }

    int RelativeMismatchIndex { get; }

    string VisibleText { get; }

    string LeftSegment { get; }
}

public record struct TextSpan
    : ITextSpan
{
    public int Start { get; init; }

    public int End { get; init; }

    public string Text { get; init; }

    public int MismatchIndex { get; init; }

    public int RelativeMismatchIndex => MismatchIndex - Start;

    public string VisibleText => Text[Start..End];

    public string LeftSegment => VisibleText[..RelativeMismatchIndex];
}
