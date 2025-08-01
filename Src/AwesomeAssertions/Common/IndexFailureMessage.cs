namespace AwesomeAssertions.Common;

internal class IndexFailureMessage
{
    public required string ExpectationDescription { get; set; }

    public required string LocationDescription { get; set; }

    public required string MismatchSegment { get; set; }

    public override string ToString()
    {
        return $$"""
                 {{ExpectationDescription}}the same string{reason}, but they differ {{LocationDescription}}:
                 {{MismatchSegment}}.
                 """;
    }

    public static implicit operator string(IndexFailureMessage message) => message.ToString();
}
