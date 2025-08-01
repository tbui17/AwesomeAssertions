using System.Collections.Generic;
using System.Linq;

namespace AwesomeAssertions.Common;

internal class TextSegment
{
    public int Indent { get; set; } = 0;

    public List<TextLine> Lines { get; set; } = new();

    public void AlignRight()
    {
        var topLine = Lines.OrderByDescending(x => x.Text.Length).First();

        foreach (var textLine in Lines.Where(l => l != topLine))
        {
            textLine.LeftPadding = topLine.Text.Length - textLine.Text.Length;
        }
    }
}
