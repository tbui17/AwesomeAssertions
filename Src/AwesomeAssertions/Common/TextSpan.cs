namespace AwesomeAssertions.Common;

public class TextSpan
{
    public int Start { get; set; }

    public int End { get; set; }

    public string Text { get; set; }

    public string VisibleText => Text[Start..End];
}
