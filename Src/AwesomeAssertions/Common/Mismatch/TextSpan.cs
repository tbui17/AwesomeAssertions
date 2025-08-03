namespace AwesomeAssertions.Common.Mismatch;

internal readonly record struct TextSpan()
    : ITextSpan
{
    public int Start { get; init; }

    public int End { get; init; }

    public string Text { get; init; } = "";

    public string VisibleText => Text.Length is 0
        ? ""
        : Text[Start..End];

    public bool StartElided => Start > 0;

    public bool EndElided => End < Text.Length;

    public int Length => End - Start;
}
