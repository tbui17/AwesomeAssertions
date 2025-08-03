using System;
using System.Collections.Generic;
using System.Linq;

namespace AwesomeAssertions.Common.Mismatch;

internal class TextSegment
{
    public List<TextLine> Lines { get; set; } = new();

    public void AlignRight()
    {
        var topLine = Lines.OrderByDescending(x => x.Text.Length).First();

        foreach (var textLine in Lines.Where(x => x != topLine))
        {
            textLine.Indent = topLine.Text.Length - textLine.Text.Length;
        }
    }

    public override string ToString() => Lines.Select(x => x.Text.PadLeft(x.Length)).Join(Environment.NewLine).EscapePlaceholders();

    public static implicit operator string(TextSegment segment) => segment.ToString();
}
