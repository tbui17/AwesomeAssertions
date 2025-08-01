namespace AwesomeAssertions.Common;

internal class ElisionConfiguration
{
    public int DefaultCharactersToKeep { get; set; } = 10;

    public int MinCharactersToKeep { get; set; } = 5;

    public int MaxCharactersToKeep { get; set; } = 15;

    public int StringPrintLength { get; set; } = AssertionConfiguration.Current.Formatting.StringPrintLength;

    private int? minStringPrintLength;

    public int MinStringPrintLength
    {
        get => minStringPrintLength ?? StringPrintLength - 5;
        set => minStringPrintLength = value;
    }

    private int? maxStringPrintLength;

    public int MaxStringPrintLength
    {
        get => maxStringPrintLength ?? StringPrintLength + 10;
        set => maxStringPrintLength = value;
    }
}
