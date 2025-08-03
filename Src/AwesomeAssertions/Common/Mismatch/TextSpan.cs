using System;

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

    public int StartOffset => Start;

    public int EndOffset => Text.Length - End;
}

internal class ReversedTextSpan(TextSpan textSpan)
    : ITextSpan
{
    public int Start => GetIndexAfterReversal(textSpan.Length, textSpan.End);

    public int End => GetIndexAfterReversal(textSpan.Length, textSpan.Start);

    public string Text => textSpan.Text;

    public string VisibleText => textSpan.Text.Length is 0
        ? ""
        : Text[Start..End];

    public bool StartElided => Start > 0;

    public bool EndElided => End < Text.Length;

    public int Length => End - Start;

    public static int GetIndexAfterReversal(int stringLength, int originalIndex) => Math.Max(stringLength - originalIndex - 1, 0);
}
