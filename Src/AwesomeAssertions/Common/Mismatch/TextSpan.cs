namespace AwesomeAssertions.Common.Mismatch;

internal interface ITextSpan
{
    int Start { get; }

    int End { get; }

    string VisibleText { get; }

    bool StartElided { get; }

    bool EndElided { get; }

    int Length { get; }
}

internal record struct TextSpan
    : ITextSpan
{
    public int Start { get; init; }

    public int End { get; init; }

    public string Text { get; init; }

    public string VisibleText => Text[Start..End];

    public bool StartElided => Start > 0;

    public bool EndElided => End < Text.Length;

    public int Length => End - Start;
}
