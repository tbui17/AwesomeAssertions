namespace AwesomeAssertions.Common.Mismatch;

internal class TruncationOptions
{
    public int DefaultCharactersToKeep { get; } = 10;

    public int MinCharactersToKeep { get; } = 5;

    public int MaxCharactersToKeep { get; } = 15;

    public int StringPrintLength { get; } = AssertionConfiguration.Current.Formatting.StringPrintLength;

    public int MinStringPrintLength => StringPrintLength - 5;

    public int MaxStringPrintLength => StringPrintLength + 10;
}
