using System;
using System.Linq;

namespace AwesomeAssertions.Common;

internal class MismatchEntry
{
    private ElisionConfiguration ElisionConfiguration { get; } = new();

    private const int LengthOfWhitespace = 1;

    public required string Text { get; init; }

    public required int MismatchIndex { get; init; }

    public bool StartElided => GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex() > 0;

    public bool EndElided => Text.Length > GetTextSpan().End;

    private TextSpan GetTextSpan()
    {
        int trimStart = GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex();
        int subjectLength = GetLengthOfPhraseToShowOrDefaultLength(Text[trimStart..]);

        return new TextSpan
        {
            Text = Text,
            Start = trimStart,
            End = trimStart + subjectLength
        };
    }

    // TODO: ???
    private string VisibleText2 => Text[GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex()..MismatchIndex];

    public string VisibleText => GetTextSpan().VisibleText;

    private int NewlineCharacterCount => VisibleText2.Count(c => c is '\r' or '\n');

    // TODO: name
    public int WhitespaceCount
    {
        get
        {
            int visibleTextLengthBeforeMismatch = MismatchIndex - GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex();
            int count = visibleTextLengthBeforeMismatch + NewlineCharacterCount;
            return StartElided ? count + 1 : count;
        }
    }

    private int GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex()
    {
        // if the slice of interest is small enough, we don't need to elide the start of the string
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
            .LastIndexOf(' ', Math.Min(ElisionConfiguration.MaxStringPrintLength + LengthOfWhitespace, value.Length) - 1);

        if (indexOfWordBoundary >= ElisionConfiguration.MinStringPrintLength)
        {
            return indexOfWordBoundary;
        }

        return Math.Min(ElisionConfiguration.StringPrintLength, value.Length);
    }
}
