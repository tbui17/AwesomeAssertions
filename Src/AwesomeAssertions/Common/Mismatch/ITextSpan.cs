namespace AwesomeAssertions.Common.Mismatch;

internal interface ITextSpan
{
    int Start { get; }

    int End { get; }

    string VisibleText { get; }

    bool StartElided { get; }

    bool EndElided { get; }

    int Length { get; }
}

internal interface IMismatchTextSpan : ITextSpan
{
    int MismatchIndex { get; }
}
