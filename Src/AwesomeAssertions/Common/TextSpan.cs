namespace AwesomeAssertions.Common;

public class TextSpan
{
    public int Start { get; set; }

    public int End { get; set; }

    public string OriginalText { get; set; }

    public string VisibleText => OriginalText[Start..End];
}
