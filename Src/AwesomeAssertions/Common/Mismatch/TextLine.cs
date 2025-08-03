namespace AwesomeAssertions.Common.Mismatch;

internal class TextLine
{
    public string Text { get; set; } = "";

    public int Indent { get; set; } = 0;

    public int Length => Indent + Text.Length;
}
