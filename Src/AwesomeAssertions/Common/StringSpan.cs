using System;

namespace AwesomeAssertions.Common;

public class StringSpan
{
    public StringSpan(string text)
    {
        Text = text;
        Start = 0;
        End = Math.Max(0, text.Length - 1);
    }

    public string Text { get; set; }
    public int Start { get; set; }
    public int End { get; set; }
    public int Length => End - Start;
}
