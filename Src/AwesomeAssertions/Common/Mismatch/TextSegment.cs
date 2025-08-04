using System;
using System.Collections.Generic;
using System.Linq;

namespace AwesomeAssertions.Common.Mismatch;

internal class TextSegment
{
    public List<TextLine> Lines { get; set; } = new();

    public override string ToString() => Lines.Select(x => x.Text.PadLeft(x.Length)).Join(Environment.NewLine).EscapePlaceholders();

    public static implicit operator string(TextSegment segment) => segment.ToString();
}
