using System;
using System.Linq;

namespace AwesomeAssertions.Common;


internal class MismatchEntry : ITextSpan
{
    private ElisionConfiguration ElisionConfiguration { get; init; } = new();

    private const int LengthOfWhitespace = 1;

    public required string Text { get; init; }

    public required int MismatchIndex { get; init; }

    private int? start;
    public int Start => start ??= GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex();

    public int End => Start + GetLengthOfPhraseToShowOrDefaultLength(Text);

    public int RelativeMismatchIndex => MismatchIndex - Start;

    public string LeftSegment => VisibleText[..RelativeMismatchIndex];

    public string VisibleText => Text[Start..End];

    public bool StartElided => Start > 0;

    public bool EndElided => Text.Length > End;

    public string VisibleTextEscaped => Text[Start..End].EscapeNewLines();

    // TODO: name
    public int WhitespaceCount
    {
        get
        {
            // compensate for newlines
            int count = LeftSegment.Length + LeftSegment.Count(c => c is '\r' or '\n');

            return StartElided ? count + 1 : count;
        }
    }

    private int GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex()
    {
        // if the slice of interest is small enough, we do not need to elide the start of the string
        if (MismatchIndex <= ElisionConfiguration.DefaultCharactersToKeep)
        {
            return 0;
        }

        int indexToStartSearchingForWordBoundary =
            Math.Max(MismatchIndex - (ElisionConfiguration.MaxCharactersToKeep + LengthOfWhitespace), 0);

        int phraseLengthToCheckForWordBoundary =
            (ElisionConfiguration.MaxCharactersToKeep - ElisionConfiguration.MinCharactersToKeep) + LengthOfWhitespace;

        int indexOfWordBoundary = Text
                .IndexOf(' ', indexToStartSearchingForWordBoundary, phraseLengthToCheckForWordBoundary) -
            indexToStartSearchingForWordBoundary;

        if (indexOfWordBoundary >= 0)
        {
            return indexToStartSearchingForWordBoundary + indexOfWordBoundary + LengthOfWhitespace;
        }

        return MismatchIndex - ElisionConfiguration.DefaultCharactersToKeep;
    }

    private int GetLengthOfPhraseToShowOrDefaultLength(string value)
    {
        int indexOfWordBoundary = value
            .LastIndexOf(' ',
                Math.Min(Start + ElisionConfiguration.MaxStringPrintLength + LengthOfWhitespace, value.Length) - 1);

        if (indexOfWordBoundary >= (Start + ElisionConfiguration.MinStringPrintLength))
        {
            return indexOfWordBoundary - Start;
        }

        return Math.Min(ElisionConfiguration.StringPrintLength, value.Length - Start);
    }
}
