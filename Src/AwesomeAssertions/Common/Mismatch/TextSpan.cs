namespace AwesomeAssertions.Common.Mismatch;

internal record TextSpan
    : ITextSpan
{
    public required int Start { get; set; }

    public required int End { get; set; }

    public required string Text { get; set; } = "";

    public virtual string VisibleText => Text.Length is 0
        ? ""
        : Text[Start..End];

    public virtual bool StartTruncated => Start > 0;

    public virtual bool EndTruncated => End < Text.Length;

    public virtual int Length => End - Start;

    public SubTextSpan Subspan(int start, int end) => new(this, start, end);

    public SubTextSpan Subspan(SpanRange range) => new(this, range.Start, range.End);

    public static TextSpan Create(string text)
    {
        return new TextSpan
        {
            Text = text,
            Start = 0,
            End = text.Length
        };
    }
}

internal class ReverseTextSpan(ITextSpan textSpan) : ITextSpan
{
    public int Start => MirrorIndex(textSpan.Length, textSpan.End);

    public int End => MirrorIndex(textSpan.Length, textSpan.Start);

    public string VisibleText => textSpan.VisibleText;

    public bool StartTruncated => textSpan.EndTruncated;

    public bool EndTruncated => textSpan.StartTruncated;

    public int Length => textSpan.Length;

    private static int MirrorIndex(int stringLength, int originalIndex) => stringLength - originalIndex - 1;
}

internal class SubTextSpan(TextSpan textSpan, int start, int end) : ITextSpan
{
    public int AbsoluteStart => textSpan.Start + Start;

    public int AbsoluteEnd => textSpan.Start + End;

    public int Start => start;

    public int End => end;

    public string VisibleText => textSpan.Text[Start..End];

    public bool StartTruncated => AbsoluteStart > 0;

    public bool EndTruncated => AbsoluteEnd < textSpan.Text.Length;

    public int Length => End - Start;
}
