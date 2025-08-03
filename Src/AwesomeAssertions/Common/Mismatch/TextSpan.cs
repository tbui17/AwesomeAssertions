namespace AwesomeAssertions.Common.Mismatch;

internal record struct TextSpan
{
    public int Start { get; init; }

    public int End { get; init; }

    public string Text { get; init; }

    public string VisibleText => Text[Start..End];

    public bool StartElided => Start > 0;

    public bool EndElided => End < Text.Length;

    public int Length => End - Start;
}
